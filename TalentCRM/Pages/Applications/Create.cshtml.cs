using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TalentCRM.Models;
using TalentCRM.Services;

namespace TalentCRM.Pages.Applications;

public class CreateModel : PageModel
{
    private readonly ApplicationService _applications;
    private readonly CandidateService _candidates;
    private readonly JobService _jobs;

    public CreateModel(ApplicationService applications, CandidateService candidates, JobService jobs)
    {
        _applications = applications;
        _candidates = candidates;
        _jobs = jobs;
    }

    // This form only needs 3 values, so instead of binding a whole JobApplication
    // we bind a small "input model" class (defined at the bottom of this file).
    // That way nobody can post e.g. Stage=Hired and skip the pipeline.
    [BindProperty]
    public NewApplicationInput Input { get; set; } = new();

    public List<SelectListItem> CandidateOptions { get; private set; } = [];
    public List<SelectListItem> JobOptions { get; private set; } = [];

    // /Applications/Create?candidateId=3&jobId=1 pre-selects the dropdowns.
    public void OnGet(int? candidateId, int? jobId)
    {
        Input.CandidateId = candidateId ?? 0;   // 📘 C#: ?? gives a fallback for null
        Input.JobId = jobId ?? 0;
        LoadOptions();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            LoadOptions();
            return Page();
        }

        try
        {
            JobApplication created = await _applications.CreateAsync(Input.CandidateId, Input.JobId, Input.Notes);
            TempData["Message"] = "Application created.";
            return RedirectToPage("Details", new { id = created.Id });
        }
        catch (Exception ex) when (ex is BusinessRuleException or NotFoundException)   // 📘 C#: exception filter
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            LoadOptions();
            return Page();
        }
    }

    private void LoadOptions()
    {
        CandidateOptions = _candidates.Search(search: null, status: null)
            .Where(r => r.Candidate.Status != CandidateStatus.NotLooking)
            .Select(r => new SelectListItem($"{r.Candidate.FullName} ({r.Candidate.Email})", r.Candidate.Id.ToString()))
            .ToList();

        JobOptions = _jobs.GetOpenJobs()
            .Select(r => new SelectListItem($"{r.Job.Title} · {r.Client.CompanyName}", r.Job.Id.ToString()))
            .ToList();
    }
}

// The "input model": just the fields this form posts.
public class NewApplicationInput
{
    [Range(1, int.MaxValue, ErrorMessage = "Choose a candidate.")]
    [Display(Name = "Candidate")]
    public int CandidateId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Choose a job.")]
    [Display(Name = "Job")]
    public int JobId { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }
}

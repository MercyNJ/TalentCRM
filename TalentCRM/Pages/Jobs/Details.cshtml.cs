using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TalentCRM.Models;
using TalentCRM.Services;

namespace TalentCRM.Pages.Jobs;

public class DetailsModel : PageModel
{
    private readonly JobService _jobs;
    private readonly CandidateService _candidates;

    public DetailsModel(JobService jobs, CandidateService candidates)
    {
        _jobs = jobs;
        _candidates = candidates;
    }

    public JobRow Row { get; private set; } = null!;
    public List<ApplicationRow> Applicants { get; private set; } = [];

    // Available candidates who have at least one of the job's required skills and haven't applied yet.
    public List<(CandidateRow Row, int MatchingSkills)> Suggestions { get; private set; } = [];

    public IActionResult OnGet(int id)
    {
        var details = _jobs.GetDetails(id);
        if (details is null) return NotFound();

        (Row, Applicants) = details.Value;

        // 📘 C#: LINQ over the results of two services: filter, score, sort, take the top 5.
        HashSet<int> alreadyApplied = Applicants.Select(a => a.Candidate.Id).ToHashSet();
        IReadOnlyList<string> required = Row.Job.RequiredSkillList;

        Suggestions = _candidates.Search(search: null, status: CandidateStatus.Available)
            .Where(c => !alreadyApplied.Contains(c.Candidate.Id))
            .Select(c => (Row: c, MatchingSkills: required.Count(skill => c.Candidate.HasSkill(skill))))
            .Where(x => x.MatchingSkills > 0)
            .OrderByDescending(x => x.MatchingSkills)
            .ThenByDescending(x => x.Row.Candidate.YearsOfExperience)
            .Take(5)
            .ToList();

        return Page();
    }
}

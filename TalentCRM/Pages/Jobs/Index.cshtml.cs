using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TalentCRM.Models;
using TalentCRM.Services;

namespace TalentCRM.Pages.Jobs;

public class IndexModel : PageModel
{
    private readonly JobService _jobs;

    public IndexModel(JobService jobs) => _jobs = jobs;

    [BindProperty(SupportsGet = true)]
    public JobStatus? Status { get; set; }

    public List<JobRow> Jobs { get; private set; } = [];

    public void OnGet() => Jobs = _jobs.Search(Status, clientId: null);
}

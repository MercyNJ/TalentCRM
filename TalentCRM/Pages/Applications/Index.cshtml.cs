using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TalentCRM.Models;
using TalentCRM.Services;

namespace TalentCRM.Pages.Applications;

public class IndexModel : PageModel
{
    private readonly ApplicationService _applications;

    public IndexModel(ApplicationService applications) => _applications = applications;

    [BindProperty(SupportsGet = true)]
    public ApplicationStage? Stage { get; set; }

    public List<ApplicationRow> Applications { get; private set; } = [];
    public Dictionary<ApplicationStage, int> Counts { get; private set; } = [];

    public void OnGet()
    {
        Applications = _applications.Search(Stage);
        Counts = _applications.CountByStage();
    }
}

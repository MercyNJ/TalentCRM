using Microsoft.AspNetCore.Mvc.RazorPages;
using TalentCRM.Services;

namespace TalentCRM.Pages;

// Loads the dashboard data displayed on the home page.
public class IndexModel : PageModel
{
    private readonly DashboardService _dashboard;

    public IndexModel(DashboardService dashboard) => _dashboard = dashboard;

    public Dashboard Dashboard { get; private set; } = null!;

    // Loads dashboard data before the page renders.
    public void OnGet()
    {
        Dashboard = _dashboard.Build();
    }
}

using Microsoft.AspNetCore.Mvc.RazorPages;
using TalentCRM.Services;

namespace TalentCRM.Pages;

// 🆕 NEW (web): a Razor Page = two files that work as a pair.
//   Index.cshtml     the HTML (with a little C# mixed in)
//   Index.cshtml.cs  this class, the "PageModel": it loads the data the HTML shows
// The URL "/" (or "/Index") runs OnGet() below, then renders Index.cshtml.
public class IndexModel : PageModel
{
    private readonly DashboardService _dashboard;

    public IndexModel(DashboardService dashboard) => _dashboard = dashboard;

    // Anything public here can be used in the .cshtml file as Model.Dashboard
    public Dashboard Dashboard { get; private set; } = null!;   // set in OnGet, before the page renders

    // 🆕 NEW (web): OnGet runs when the browser GETs (opens) this page.
    public void OnGet()
    {
        Dashboard = _dashboard.Build();
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TalentCRM.Services;

namespace TalentCRM.Pages.Interviews;

public class IndexModel : PageModel
{
    private readonly InterviewService _interviews;

    public IndexModel(InterviewService interviews) => _interviews = interviews;

    // /Interviews shows upcoming only. /Interviews?all=true shows everything.
    [BindProperty(SupportsGet = true)]
    public bool All { get; set; }

    public List<InterviewRow> Interviews { get; private set; } = [];

    // Interviews grouped by day, e.g. "Monday 29 September" -> [interviews that day]
    // 📘 C#: GroupBy returns IGrouping<TKey, TElement>, a key plus the items that share it.
    public List<IGrouping<DateTime, InterviewRow>> ByDay { get; private set; } = [];

    public void OnGet()
    {
        Interviews = _interviews.Search(upcomingOnly: !All);

        var ordered = All
            ? Interviews.OrderByDescending(i => i.Interview.ScheduledAt)   // history: newest first
            : Interviews.OrderBy(i => i.Interview.ScheduledAt);            // upcoming: soonest first

        ByDay = ordered.GroupBy(i => i.Interview.ScheduledAt.Date).ToList();
    }
}

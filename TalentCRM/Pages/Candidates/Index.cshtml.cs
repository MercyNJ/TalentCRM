using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TalentCRM.Models;
using TalentCRM.Services;

namespace TalentCRM.Pages.Candidates;

public class IndexModel : PageModel
{
    private readonly CandidateService _candidates;

    public IndexModel(CandidateService candidates) => _candidates = candidates;

    // 🆕 NEW (web): MODEL BINDING. [BindProperty] fills this property from the request automatically.
    // SupportsGet = true means "also from the URL", e.g. /Candidates?search=react&status=Available
    // (By default [BindProperty] only binds on POST, i.e. when a form is submitted.)
    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    [BindProperty(SupportsGet = true)]
    public CandidateStatus? Status { get; set; }

    public List<CandidateRow> Candidates { get; private set; } = [];

    public void OnGet()
    {
        Candidates = _candidates.Search(Search, Status);
    }
}

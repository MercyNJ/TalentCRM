using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TalentCRM.Models;
using TalentCRM.Services;

namespace TalentCRM.Pages.Candidates;

public class DetailsModel : PageModel
{
    private readonly CandidateService _candidates;

    public DetailsModel(CandidateService candidates) => _candidates = candidates;

    public Candidate Candidate { get; private set; } = null!;
    public List<ApplicationRow> Applications { get; private set; } = [];

    // `id` comes from the URL: /Candidates/Details/3 (see `@page "{id:int}"` in the .cshtml)
    public IActionResult OnGet(int id)
    {
        var profile = _candidates.GetProfile(id);
        if (profile is null)
        {
            return NotFound();
        }

        // 📘 C#: tuple deconstruction. Unpack both values in one line.
        (Candidate, Applications) = profile.Value;
        return Page();
    }

    // 🆕 NEW (web): a NAMED handler. The Delete button posts with asp-page-handler="Delete",
    // which runs OnPostDeleteAsync instead of OnPostAsync. One page can have several buttons this way.
    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        try
        {
            await _candidates.DeleteAsync(id);
            TempData["Message"] = "Candidate deleted.";
            return RedirectToPage("Index");
        }
        catch (BusinessRuleException ex)
        {
            TempData["Message"] = ex.Message;
            return RedirectToPage("Details", new { id });
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }
}

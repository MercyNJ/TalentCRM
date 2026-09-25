using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TalentCRM.Models;
using TalentCRM.Services;

namespace TalentCRM.Pages.Candidates;

// ONE page for both "new candidate" and "edit candidate":
//   /Candidates/Edit      no id: a new, empty form
//   /Candidates/Edit/3    editing candidate 3
// (See `@page "{id:int?}"` at the top of Edit.cshtml. The `?` makes the id optional.)
public class EditModel : PageModel
{
    private readonly CandidateService _candidates;

    public EditModel(CandidateService candidates) => _candidates = candidates;

    // When the form is POSTed, ASP.NET Core fills this object from the form fields
    // (FirstName, Email, ...) and checks the [Required]/[EmailAddress]... rules.
    [BindProperty]
    public Candidate Candidate { get; set; } = new();

    public bool IsNew => Candidate.Id == 0;

    // 🆕 NEW (web): IActionResult means "what should the browser get back?"
    //   Page()            show this page
    //   NotFound()        a 404 "not found" response
    //   RedirectToPage()  send the browser to another page
    public IActionResult OnGet(int? id)
    {
        if (id is null)
        {
            return Page();   // new candidate: empty form
        }

        Candidate? existing = _candidates.Find(id.Value);
        if (existing is null)
        {
            return NotFound();
        }

        Candidate = existing;
        return Page();
    }

    // 🆕 NEW (web): OnPostAsync runs when the form is submitted (method="post").
    // 📘 C#: async Task<IActionResult>, because saving to the file is async.
    public async Task<IActionResult> OnPostAsync()
    {
        // 🆕 NEW (web): ModelState holds the validation results for everything that was posted.
        if (!ModelState.IsValid)
        {
            return Page();   // show the form again, with the error messages
        }

        try
        {
            if (IsNew)
            {
                Candidate created = await _candidates.AddAsync(Candidate);
                TempData["Message"] = $"{created.FullName} was added.";
                return RedirectToPage("Details", new { id = created.Id });
            }

            await _candidates.UpdateAsync(Candidate);
            TempData["Message"] = "Changes saved.";
            return RedirectToPage("Details", new { id = Candidate.Id });
        }
        catch (BusinessRuleException ex)
        {
            // A rule was broken (e.g. duplicate email). Show it at the top of the form.
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }
    // 🆕 NEW (web): "Post/Redirect/Get". After a successful POST we REDIRECT instead of showing a page.
    // That way pressing F5 afterwards doesn't submit the form a second time.
}

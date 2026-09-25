using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TalentCRM.Models;
using TalentCRM.Services;

namespace TalentCRM.Pages.Interviews;

//   /Interviews/Edit?applicationId=4   schedule a NEW interview for application 4
//   /Interviews/Edit/7                 reschedule interview 7, or record its outcome
public class EditModel : PageModel
{
    private readonly InterviewService _interviews;

    public EditModel(InterviewService interviews) => _interviews = interviews;

    [BindProperty]
    public Interview Interview { get; set; } = new();

    // Who/what the interview is for. Shown in the heading, and NOT posted back.
    public ApplicationRow? For { get; private set; }

    public bool IsNew => Interview.Id == 0;

    public IActionResult OnGet(int? id, int? applicationId)
    {
        if (id is not null)
        {
            Interview? existing = _interviews.Find(id.Value);
            if (existing is null) return NotFound();
            Interview = existing;
        }
        else if (applicationId is not null)
        {
            // Sensible defaults: tomorrow at 10:00.
            Interview.ApplicationId = applicationId.Value;
            Interview.ScheduledAt = DateTime.Today.AddDays(1).AddHours(10);
        }
        else
        {
            return RedirectToPage("Index");   // nothing to schedule for
        }

        For = _interviews.GetApplicationRow(Interview.ApplicationId);
        return For is null ? NotFound() : Page();   // 📘 C#: the conditional (ternary) operator
    }

    public async Task<IActionResult> OnPostAsync()
    {
        For = _interviews.GetApplicationRow(Interview.ApplicationId);
        if (For is null) return NotFound();

        if (!ModelState.IsValid) return Page();

        try
        {
            if (IsNew)
            {
                await _interviews.ScheduleAsync(Interview);
                TempData["Message"] = $"Interview scheduled for {Interview.ScheduledAt:dddd d MMM 'at' HH:mm}.";
            }
            else
            {
                await _interviews.UpdateAsync(Interview);
                TempData["Message"] = "Interview updated.";
            }

            return RedirectToPage("/Applications/Details", new { id = Interview.ApplicationId });
        }
        catch (BusinessRuleException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }
}

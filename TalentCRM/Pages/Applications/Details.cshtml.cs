using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TalentCRM.Models;
using TalentCRM.Services;

namespace TalentCRM.Pages.Applications;

public class DetailsModel : PageModel
{
    private readonly ApplicationService _applications;

    public DetailsModel(ApplicationService applications) => _applications = applications;

    public ApplicationRow Row { get; private set; } = null!;
    public List<Interview> Interviews { get; private set; } = [];

    // The buttons we show: only the moves the pipeline allows from the current stage.
    public IReadOnlyList<ApplicationStage> NextStages => ApplicationService.AllowedNextStages(Row.Application.Stage);
    public bool CanSchedule => ApplicationService.CanScheduleInterview(Row.Application.Stage);

    // The "happy path" stages, for the progress bar at the top of the page.
    public static readonly ApplicationStage[] PipelineSteps =
    [
        ApplicationStage.Applied, ApplicationStage.Screening, ApplicationStage.Interviewing,
        ApplicationStage.Offered, ApplicationStage.Hired
    ];

    public IActionResult OnGet(int id)
    {
        var details = _applications.GetDetails(id);
        if (details is null) return NotFound();

        (Row, Interviews) = details.Value;
        return Page();
    }

    // Each stage button posts   handler=Move  and  stage=Offered (for example).
    // Model binding turns the text "Offered" into the enum value ApplicationStage.Offered.
    public async Task<IActionResult> OnPostMoveAsync(int id, ApplicationStage stage)
    {
        try
        {
            TempData["Message"] = await _applications.MoveToStageAsync(id, stage);
        }
        catch (BusinessRuleException ex)
        {
            TempData["Message"] = ex.Message;
        }
        catch (NotFoundException)
        {
            return NotFound();
        }

        return RedirectToPage("Details", new { id });
    }
}

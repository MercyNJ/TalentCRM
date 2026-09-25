using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TalentCRM.Models;
using TalentCRM.Services;

namespace TalentCRM.Pages.Clients;

public class DetailsModel : PageModel
{
    private readonly ClientService _clients;

    public DetailsModel(ClientService clients) => _clients = clients;

    public Client Client { get; private set; } = null!;
    public List<JobRow> Jobs { get; private set; } = [];

    public IActionResult OnGet(int id)
    {
        var details = _clients.GetDetails(id);
        if (details is null) return NotFound();

        (Client, Jobs) = details.Value;
        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        try
        {
            await _clients.DeleteAsync(id);
            TempData["Message"] = "Client deleted.";
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

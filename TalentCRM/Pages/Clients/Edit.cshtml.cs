using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TalentCRM.Models;
using TalentCRM.Services;

namespace TalentCRM.Pages.Clients;

// Same pattern as Candidates/Edit: one page for "new" (no id) and "edit" (with id).
public class EditModel : PageModel
{
    private readonly ClientService _clients;

    public EditModel(ClientService clients) => _clients = clients;

    [BindProperty]
    public Client Client { get; set; } = new();

    public bool IsNew => Client.Id == 0;

    public IActionResult OnGet(int? id)
    {
        if (id is null) return Page();

        Client? existing = _clients.Find(id.Value);
        if (existing is null) return NotFound();

        Client = existing;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        try
        {
            if (IsNew)
            {
                Client created = await _clients.AddAsync(Client);
                TempData["Message"] = $"{created.CompanyName} was added.";
                return RedirectToPage("Details", new { id = created.Id });
            }

            await _clients.UpdateAsync(Client);
            TempData["Message"] = "Changes saved.";
            return RedirectToPage("Details", new { id = Client.Id });
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

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TalentCRM.Models;
using TalentCRM.Services;

namespace TalentCRM.Pages.Jobs;

public class EditModel : PageModel
{
    private readonly JobService _jobs;
    private readonly ClientService _clients;

    // A page can ask for as many services as it needs.
    public EditModel(JobService jobs, ClientService clients)
    {
        _jobs = jobs;
        _clients = clients;
    }

    [BindProperty]
    public Job Job { get; set; } = new();

    public bool IsNew => Job.Id == 0;

    // Options for the "Client" dropdown.
    public List<SelectListItem> ClientOptions { get; private set; } = [];

    // /Jobs/Edit?clientId=2 pre-selects a client (used by the "+ New job" button on a client's page).
    public IActionResult OnGet(int? id, int? clientId)
    {
        if (id is not null)
        {
            Job? existing = _jobs.Find(id.Value);
            if (existing is null) return NotFound();
            Job = existing;
        }
        else if (clientId is not null)
        {
            Job.ClientId = clientId.Value;
        }

        LoadClientOptions();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            LoadClientOptions();   // dropdown options aren't posted back, so rebuild them
            return Page();
        }

        try
        {
            if (IsNew)
            {
                Job created = await _jobs.AddAsync(Job);
                TempData["Message"] = $"\"{created.Title}\" was created.";
                return RedirectToPage("Details", new { id = created.Id });
            }

            await _jobs.UpdateAsync(Job);
            TempData["Message"] = "Changes saved.";
            return RedirectToPage("Details", new { id = Job.Id });
        }
        catch (BusinessRuleException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            LoadClientOptions();
            return Page();
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    private void LoadClientOptions() =>
        ClientOptions = _clients.GetAll()
            .Select(r => new SelectListItem(r.Client.CompanyName, r.Client.Id.ToString()))
            .ToList();
}

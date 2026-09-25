using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TalentCRM.Services;

namespace TalentCRM.Pages.Clients;

public class IndexModel : PageModel
{
    private readonly ClientService _clients;

    public IndexModel(ClientService clients) => _clients = clients;

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    public List<ClientRow> Clients { get; private set; } = [];

    public void OnGet() => Clients = _clients.GetAll(Search);
}

using TalentCRM.Data;
using TalentCRM.Models;

namespace TalentCRM.Services;

public class ClientService
{
    private readonly ITalentStore _store;

    public ClientService(ITalentStore store) => _store = store;

    public List<ClientRow> GetAll(string? search = null) =>
        _store.Read(data => data.Clients
            .Where(c => string.IsNullOrWhiteSpace(search)
                     || c.CompanyName.ContainsText(search.Trim())
                     || c.Industry.ContainsText(search.Trim())
                     || c.Location.ContainsText(search.Trim()))
            .OrderBy(c => c.CompanyName)
            .Select(c => new ClientRow(
                c,
                OpenJobs: data.Jobs.Count(j => j.ClientId == c.Id && j.Status == JobStatus.Open),
                TotalJobs: data.Jobs.Count(j => j.ClientId == c.Id)))
            .ToList());

    public Client? Find(int id) =>
        _store.Read(data => data.Clients.FirstOrDefault(c => c.Id == id));

    public (Client Client, List<JobRow> Jobs)? GetDetails(int id) =>
        _store.Read<(Client, List<JobRow>)?>(data =>
        {
            Client? client = data.Clients.FirstOrDefault(c => c.Id == id);
            if (client is null) return null;

            List<JobRow> jobs = data.Jobs
                .Where(j => j.ClientId == id)
                .OrderBy(j => j.Status).ThenByDescending(j => j.CreatedAt) 
                .Select(j => JobService.BuildRow(j, client, data))
                .ToList();

            return (client, jobs);
        });

    public Task<Client> AddAsync(Client client) =>
        _store.WriteAsync(data =>
        {
            EnsureNameIsUnique(data, client.CompanyName, ignoreId: 0);

            client.Id = data.Clients.NextId();
            client.CreatedAt = DateTime.Now;
            data.Clients.Add(client);
            return client;
        });

    public Task UpdateAsync(Client changes) =>
        _store.WriteAsync(data =>
        {
            Client existing = data.Clients.GetOrThrow(changes.Id);
            EnsureNameIsUnique(data, changes.CompanyName, ignoreId: changes.Id);

            existing.CompanyName = changes.CompanyName;
            existing.Industry = changes.Industry;
            existing.ContactName = changes.ContactName;
            existing.ContactEmail = changes.ContactEmail;
            existing.ContactPhone = changes.ContactPhone;
            existing.Location = changes.Location;
        });

    public Task DeleteAsync(int id) =>
        _store.WriteAsync(data =>
        {
            Client client = data.Clients.GetOrThrow(id);

            int jobCount = data.Jobs.Count(j => j.ClientId == id);
            if (jobCount > 0)
            {
                throw new BusinessRuleException(
                    $"{client.CompanyName} has {jobCount} job(s). Close them instead. Clients with job history can't be deleted.");
            }

            data.Clients.Remove(client);
        });

    private static void EnsureNameIsUnique(TalentData data, string name, int ignoreId)
    {
        if (data.Clients.Any(c => c.Id != ignoreId && c.CompanyName.Equals(name.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            throw new BusinessRuleException($"A client called \"{name}\" already exists.");
        }
    }
}

using TalentCRM.Data;
using TalentCRM.Models;

namespace TalentCRM.Services;

public class JobService
{
    private readonly ITalentStore _store;

    public JobService(ITalentStore store) => _store = store;

    public List<JobRow> Search(JobStatus? status, int? clientId) =>
        _store.Read(data =>
        {
            IEnumerable<Job> query = data.Jobs;

            if (status is not null) query = query.Where(j => j.Status == status);
            if (clientId is not null) query = query.Where(j => j.ClientId == clientId);

            return query
                .OrderBy(j => j.Status).ThenByDescending(j => j.CreatedAt)
                .Select(j => BuildRow(j, data.Clients.GetOrThrow(j.ClientId), data))
                .ToList();
        });

    // Open jobs only, for the "which job?" dropdown when adding an application.
    public List<JobRow> GetOpenJobs() => Search(JobStatus.Open, clientId: null);

    public Job? Find(int id) =>
        _store.Read(data => data.Jobs.FirstOrDefault(j => j.Id == id));

    public (JobRow Job, List<ApplicationRow> Applicants)? GetDetails(int id) =>
        _store.Read<(JobRow, List<ApplicationRow>)?>(data =>
        {
            Job? job = data.Jobs.FirstOrDefault(j => j.Id == id);
            if (job is null) return null;

            JobRow row = BuildRow(job, data.Clients.GetOrThrow(job.ClientId), data);
            List<ApplicationRow> applicants = data.Applications.Where(a => a.JobId == id).ToRows(data);
            return (row, applicants);
        });

    public Task<Job> AddAsync(Job job) =>
        _store.WriteAsync(data =>
        {
            Validate(data, job, hiredSoFar: 0);

            job.Id = data.Jobs.NextId();
            job.CreatedAt = DateTime.Now;
            data.Jobs.Add(job);
            return job;
        });

    public Task UpdateAsync(Job changes) =>
        _store.WriteAsync(data =>
        {
            Job existing = data.Jobs.GetOrThrow(changes.Id);
            int hired = CountHired(data, existing.Id);
            Validate(data, changes, hired);

            existing.Title = changes.Title;
            existing.ClientId = changes.ClientId;
            existing.Location = changes.Location;
            existing.EmploymentType = changes.EmploymentType;
            existing.SalaryMin = changes.SalaryMin;
            existing.SalaryMax = changes.SalaryMax;
            existing.Openings = changes.Openings;
            existing.Status = changes.Status;
            existing.Description = changes.Description;
            existing.RequiredSkills = changes.RequiredSkills;
        });

    // Shared with ClientService, so it's `internal static`.
    internal static JobRow BuildRow(Job job, Client client, TalentData data) =>
        new(job,
            client,
            Applicants: data.Applications.Count(a => a.JobId == job.Id),
            Hired: CountHired(data, job.Id));

    internal static int CountHired(TalentData data, int jobId) =>
        data.Applications.Count(a => a.JobId == jobId && a.Stage == ApplicationStage.Hired);

    private static void Validate(TalentData data, Job job, int hiredSoFar)
    {
        if (!data.Clients.Any(c => c.Id == job.ClientId))
            throw new BusinessRuleException("Please choose a valid client.");

        if (job.SalaryMin is decimal min && job.SalaryMax is decimal max && min > max)
            throw new BusinessRuleException("\"Salary from\" can't be higher than \"Salary to\".");

        if (job.Openings < hiredSoFar)
            throw new BusinessRuleException($"{hiredSoFar} people are already hired, so openings can't be less than {hiredSoFar}.");
    }
}

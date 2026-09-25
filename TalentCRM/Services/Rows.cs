using TalentCRM.Data;
using TalentCRM.Models;

namespace TalentCRM.Services;

public record ApplicationRow(JobApplication Application, Candidate Candidate, Job Job, Client Client);

public record InterviewRow(Interview Interview, ApplicationRow Row);

public record CandidateRow(Candidate Candidate, int ActiveApplications);

public record ClientRow(Client Client, int OpenJobs, int TotalJobs);

public record JobRow(Job Job, Client Client, int Applicants, int Hired);

internal static class RowBuilder
{
    public static List<ApplicationRow> ToRows(this IEnumerable<JobApplication> applications, TalentData data) =>
        (from app in applications
         join candidate in data.Candidates on app.CandidateId equals candidate.Id
         join job in data.Jobs on app.JobId equals job.Id
         join client in data.Clients on job.ClientId equals client.Id
         orderby app.StageChangedAt descending
         select new ApplicationRow(app, candidate, job, client))
        .ToList();

    public static List<InterviewRow> ToRows(this IEnumerable<Interview> interviews, TalentData data)
    {
        Dictionary<int, ApplicationRow> rowsByApplicationId = data.Applications
            .ToRows(data)
            .ToDictionary(r => r.Application.Id);

        return interviews
            .Where(i => rowsByApplicationId.ContainsKey(i.ApplicationId))
            .OrderBy(i => i.ScheduledAt)
            .Select(i => new InterviewRow(i, rowsByApplicationId[i.ApplicationId]))
            .ToList();
    }
}

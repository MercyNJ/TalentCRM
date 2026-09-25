using TalentCRM.Data;
using TalentCRM.Models;

namespace TalentCRM.Services;

public record DashboardStats(
    int Candidates,
    int AvailableCandidates,
    int Clients,
    int OpenJobs,
    int OpenPositions,
    int ActiveApplications,
    int HiresThisMonth);

public record StageCount(ApplicationStage Stage, int Count);

public record SkillCount(string Skill, int Candidates);

public record Dashboard(
    DashboardStats Stats,
    List<StageCount> Pipeline,
    List<InterviewRow> UpcomingInterviews,
    List<JobRow> JobsNeedingAttention,
    List<SkillCount> TopSkills,
    List<ApplicationRow> RecentActivity);

public class DashboardService
{
    private readonly ITalentStore _store;

    public DashboardService(ITalentStore store) => _store = store;

    public Dashboard Build() => _store.Read(data =>
    {
        DateTime now = DateTime.Now;
        var monthStart = new DateTime(now.Year, now.Month, 1);

        List<Job> openJobs = data.Jobs.Where(j => j.Status == JobStatus.Open).ToList();

        var stats = new DashboardStats(
            Candidates: data.Candidates.Count,
            AvailableCandidates: data.Candidates.Count(c => c.Status == CandidateStatus.Available),
            Clients: data.Clients.Count,
            OpenJobs: openJobs.Count,
            OpenPositions: openJobs.Sum(j => j.Openings - JobService.CountHired(data, j.Id)),
            ActiveApplications: data.Applications.Count(a => a.IsActive),
            HiresThisMonth: data.Applications.Count(a => a.Stage == ApplicationStage.Hired && a.StageChangedAt >= monthStart));

        // Pipeline: one entry per ACTIVE stage, including stages with 0 applications.
        Dictionary<ApplicationStage, int> counts = data.Applications
            .GroupBy(a => a.Stage)
            .ToDictionary(g => g.Key, g => g.Count());

        var pipeline = new List<StageCount>();
        foreach (ApplicationStage stage in Enum.GetValues<ApplicationStage>())
        {
            if (stage.IsFinal() && stage != ApplicationStage.Hired) continue;
            pipeline.Add(new StageCount(stage, counts.GetValueOrDefault(stage)));
        }

        // Next 7 days of interviews.
        List<InterviewRow> upcoming = data.Interviews
            .Where(i => i.IsUpcoming && i.ScheduledAt <= now.AddDays(7))
            .ToRows(data)
            .Take(6)
            .ToList();

        // Open jobs with fewer than 2 applicants: the recruiter should look for candidates.
        List<JobRow> needAttention = openJobs
            .Select(j => JobService.BuildRow(j, data.Clients.GetOrThrow(j.ClientId), data))
            .Where(r => r.Applicants < 2)
            .OrderBy(r => r.Applicants).ThenBy(r => r.Job.CreatedAt)
            .ToList();

        // Most common skills among AVAILABLE candidates.
        List<SkillCount> topSkills = data.Candidates
            .Where(c => c.Status == CandidateStatus.Available)
            .SelectMany(c => c.SkillList)
            .GroupBy(skill => skill, StringComparer.OrdinalIgnoreCase)
            .Select(g => new SkillCount(g.First(), g.Count()))
            .OrderByDescending(s => s.Candidates).ThenBy(s => s.Skill)
            .Take(8)
            .ToList();

        List<ApplicationRow> recent = data.Applications.ToRows(data).Take(6).ToList();

        return new Dashboard(stats, pipeline, upcoming, needAttention, topSkills, recent);
    });
}

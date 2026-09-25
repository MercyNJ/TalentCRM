using TalentCRM.Data;
using TalentCRM.Models;

namespace TalentCRM.Services;

public class ApplicationService
{
    private readonly ITalentStore _store;

    public ApplicationService(ITalentStore store) => _store = store;

    public static IReadOnlyList<ApplicationStage> AllowedNextStages(ApplicationStage current) => current switch
    {
        ApplicationStage.Applied      => [ApplicationStage.Screening, ApplicationStage.Rejected, ApplicationStage.Withdrawn],
        ApplicationStage.Screening    => [ApplicationStage.Interviewing, ApplicationStage.Rejected, ApplicationStage.Withdrawn],
        ApplicationStage.Interviewing => [ApplicationStage.Offered, ApplicationStage.Rejected, ApplicationStage.Withdrawn],
        ApplicationStage.Offered      => [ApplicationStage.Hired, ApplicationStage.Rejected, ApplicationStage.Withdrawn],
        _                             => []   // Hired, Rejected and Withdrawn are final: nothing comes after them
    };

    // Interviews can only be booked while the application is being assessed.
    public static bool CanScheduleInterview(ApplicationStage stage) =>
        stage is ApplicationStage.Screening or ApplicationStage.Interviewing;

    public List<ApplicationRow> Search(ApplicationStage? stage) =>
        _store.Read(data => data.Applications
            .Where(a => stage == null || a.Stage == stage)
            .ToRows(data));

    public Dictionary<ApplicationStage, int> CountByStage() =>
        _store.Read(data => data.Applications
            .GroupBy(a => a.Stage)
            .ToDictionary(g => g.Key, g => g.Count()));

    public (ApplicationRow Row, List<Interview> Interviews)? GetDetails(int id) =>
        _store.Read<(ApplicationRow, List<Interview>)?>(data =>
        {
            ApplicationRow? row = data.Applications.Where(a => a.Id == id).ToRows(data).FirstOrDefault();
            if (row is null) return null;

            List<Interview> interviews = data.Interviews
                .Where(i => i.ApplicationId == id)
                .OrderBy(i => i.ScheduledAt)
                .ToList();

            return (row, interviews);
        });

    public Task<JobApplication> CreateAsync(int candidateId, int jobId, string? notes) =>
        _store.WriteAsync(data =>
        {
            Candidate candidate = data.Candidates.GetOrThrow(candidateId);
            Job job = data.Jobs.GetOrThrow(jobId);

            if (job.Status != JobStatus.Open)
                throw new BusinessRuleException($"\"{job.Title}\" is {job.Status.ToDisplay().ToLower()}, so it isn't accepting applications.");

            if (candidate.Status == CandidateStatus.NotLooking)
                throw new BusinessRuleException($"{candidate.FullName} is marked as \"Not looking\". Update their profile first.");

            bool alreadyApplied = data.Applications.Any(a => a.CandidateId == candidateId && a.JobId == jobId && a.IsActive);
            if (alreadyApplied)
                throw new BusinessRuleException($"{candidate.FullName} already has an active application for \"{job.Title}\".");

            var application = new JobApplication
            {
                Id = data.Applications.NextId(),
                CandidateId = candidateId,
                JobId = jobId,
                Notes = notes,
                Stage = ApplicationStage.Applied,
                CreatedAt = DateTime.Now,
                StageChangedAt = DateTime.Now
            };

            data.Applications.Add(application);
            return application;
        });

    // Move an application to a new stage.
    public Task<string> MoveToStageAsync(int applicationId, ApplicationStage newStage) =>
        _store.WriteAsync(data =>
        {
            JobApplication application = data.Applications.GetOrThrow(applicationId);
            Candidate candidate = data.Candidates.GetOrThrow(application.CandidateId);
            Job job = data.Jobs.GetOrThrow(application.JobId);

            if (!AllowedNextStages(application.Stage).Contains(newStage))
            {
                throw new BusinessRuleException(
                    $"Can't move from {application.Stage.ToDisplay()} to {newStage.ToDisplay()}.");
            }

            // Extra checks for hiring, all done BEFORE we change anything.
            int hiredSoFar = JobService.CountHired(data, job.Id);
            if (newStage == ApplicationStage.Hired && hiredSoFar >= job.Openings)
            {
                throw new BusinessRuleException($"All {job.Openings} opening(s) for \"{job.Title}\" are already filled.");
            }

            application.Stage = newStage;
            application.StageChangedAt = DateTime.Now;
            string message = $"{candidate.FullName} moved to {newStage.ToDisplay()}.";

            // Leaving the pipeline? Cancel any interviews that haven't happened yet.
            if (newStage.IsFinal())
            {
                foreach (Interview interview in data.Interviews.Where(i => i.ApplicationId == application.Id && i.IsUpcoming))
                {
                    interview.Outcome = InterviewOutcome.Cancelled;
                    interview.Feedback = AppendNote(interview.Feedback, $"Cancelled: application {newStage.ToDisplay().ToLower()}.");
                }
            }

            if (newStage == ApplicationStage.Hired)
            {
                candidate.Status = CandidateStatus.Placed;
                message += CloseJobIfFull(data, job, hiredSoFar + 1);
            }

            return message;
        });

    // When the last opening is filled, mark the job Filled and close the other active applications.
    private static string CloseJobIfFull(TalentData data, Job job, int hiredNow)
    {
        if (hiredNow < job.Openings)
        {
            return $" {job.Openings - hiredNow} opening(s) left for this job.";
        }

        job.Status = JobStatus.Filled;

        int closed = 0;
        foreach (JobApplication other in data.Applications.Where(a => a.JobId == job.Id && a.IsActive))
        {
            other.Stage = ApplicationStage.Rejected;
            other.StageChangedAt = DateTime.Now;
            other.Notes = AppendNote(other.Notes, "Position filled.");
            closed++;
        }

        return closed == 0
            ? " The job is now filled."
            : $" The job is now filled, and {closed} other active application(s) were closed.";
    }

    private static string AppendNote(string? existing, string note) =>
        string.IsNullOrWhiteSpace(existing) ? note : $"{existing}\n{note}";
}
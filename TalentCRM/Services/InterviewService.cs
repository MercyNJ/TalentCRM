using TalentCRM.Data;
using TalentCRM.Models;

namespace TalentCRM.Services;

public class InterviewService
{
    private readonly ITalentStore _store;

    public InterviewService(ITalentStore store) => _store = store;

    public List<InterviewRow> Search(bool upcomingOnly) =>
        _store.Read(data => data.Interviews
            .Where(i => !upcomingOnly || i.IsUpcoming)
            .ToRows(data));

    public Interview? Find(int id) =>
        _store.Read(data => data.Interviews.FirstOrDefault(i => i.Id == id));

    // The candidate/job this interview (or new interview) belongs to. Used for the page heading.
    public ApplicationRow? GetApplicationRow(int applicationId) =>
        _store.Read(data => data.Applications.Where(a => a.Id == applicationId).ToRows(data).FirstOrDefault());

    public Task<Interview> ScheduleAsync(Interview interview) =>
        _store.WriteAsync(data =>
        {
            JobApplication application = data.Applications.GetOrThrow(interview.ApplicationId);

            if (!ApplicationService.CanScheduleInterview(application.Stage))
                throw new BusinessRuleException(
                    $"Interviews can only be scheduled while an application is in Screening or Interviewing (this one is {application.Stage.ToDisplay()}).");

            if (interview.ScheduledAt < DateTime.Now)
                throw new BusinessRuleException("Pick a date and time in the future.");

            EnsureNoClash(data, interview);

            interview.Id = data.Interviews.NextId();
            interview.Outcome = InterviewOutcome.Pending;
            interview.CreatedAt = DateTime.Now;
            data.Interviews.Add(interview);

            // Booking the first interview moves the application forward automatically.
            if (application.Stage == ApplicationStage.Screening)
            {
                application.Stage = ApplicationStage.Interviewing;
                application.StageChangedAt = DateTime.Now;
            }

            return interview;
        });

    public Task UpdateAsync(Interview changes) =>
        _store.WriteAsync(data =>
        {
            Interview existing = data.Interviews.GetOrThrow(changes.Id);

            switch (changes.Outcome)
            {
                case InterviewOutcome.Pending when changes.ScheduledAt < DateTime.Now:
                    throw new BusinessRuleException(
                        "This time is in the past. Pick a future time, or record the outcome (Passed / Failed / No show).");

                case InterviewOutcome.Passed or InterviewOutcome.Failed or InterviewOutcome.NoShow
                    when changes.ScheduledAt > DateTime.Now:
                    throw new BusinessRuleException("You can't record an outcome for an interview that hasn't happened yet.");
            }

            if (changes.Outcome == InterviewOutcome.Pending)
                EnsureNoClash(data, changes);

            existing.ScheduledAt = changes.ScheduledAt;
            existing.DurationMinutes = changes.DurationMinutes;
            existing.Type = changes.Type;
            existing.Interviewer = changes.Interviewer;
            existing.Outcome = changes.Outcome;
            existing.Feedback = changes.Feedback;
        });

    private static void EnsureNoClash(TalentData data, Interview interview)
    {
        int candidateId = data.Applications.GetOrThrow(interview.ApplicationId).CandidateId;

        // All applications (for any job) belonging to this candidate.
        HashSet<int> candidateApplicationIds = data.Applications
            .Where(a => a.CandidateId == candidateId)
            .Select(a => a.Id)
            .ToHashSet();

        DateTime start = interview.ScheduledAt;
        DateTime end = interview.EndsAt;

        Interview? clash = data.Interviews.FirstOrDefault(other =>
            other.Id != interview.Id
            && other.Outcome == InterviewOutcome.Pending
            && candidateApplicationIds.Contains(other.ApplicationId)
            && start < other.EndsAt && other.ScheduledAt < end);

        if (clash is not null)
            throw new BusinessRuleException(
                $"The candidate already has an interview from {clash.ScheduledAt:ddd d MMM, HH:mm} to {clash.EndsAt:HH:mm}.");
    }
}

using System.Text.RegularExpressions;

namespace TalentCRM.Models;

// Provides extension methods for enums.
public static class EnumExtensions
{
    // Converts enum names into readable text.
    public static string ToDisplay(this Enum value)
    {
        string spaced = Regex.Replace(value.ToString(), "(?<!^)([A-Z])", " $1");
        return spaced[..1] + spaced[1..].ToLower();
    }

    // Checks whether an application stage is final.
    public static bool IsFinal(this ApplicationStage stage) =>
        stage is ApplicationStage.Hired or ApplicationStage.Rejected or ApplicationStage.Withdrawn;

    // Maps application stages to CSS classes.
    public static string BadgeClass(this ApplicationStage stage) => stage switch
    {
        ApplicationStage.Applied => "badge",
        ApplicationStage.Screening or ApplicationStage.Interviewing => "badge badge-blue",
        ApplicationStage.Offered => "badge badge-amber",
        ApplicationStage.Hired => "badge badge-green",
        _ => "badge badge-muted"
    };

    public static string BadgeClass(this JobStatus status) => status switch
    {
        JobStatus.Open => "badge badge-green",
        JobStatus.OnHold => "badge badge-amber",
        JobStatus.Filled => "badge badge-blue",
        _ => "badge badge-muted"
    };

    public static string BadgeClass(this CandidateStatus status) => status switch
    {
        CandidateStatus.Available => "badge badge-green",
        CandidateStatus.Placed => "badge badge-blue",
        _ => "badge badge-muted"
    };

    public static string BadgeClass(this InterviewOutcome outcome) => outcome switch
    {
        InterviewOutcome.Passed => "badge badge-green",
        InterviewOutcome.Failed or InterviewOutcome.NoShow => "badge badge-red",
        InterviewOutcome.Cancelled => "badge badge-muted",
        _ => "badge"
    };
}
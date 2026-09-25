using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TalentCRM.Models;

// Represents an interview for an application.
public class Interview : Entity
{
    public int ApplicationId { get; set; }

    [Display(Name = "Date and time")]
    public DateTime ScheduledAt { get; set; }

    [Range(15, 480)]
    [Display(Name = "Duration (minutes)")]
    public int DurationMinutes { get; set; } = 60;

    public InterviewType Type { get; set; } = InterviewType.Video;

    [Required, StringLength(80)]
    public string Interviewer { get; set; } = string.Empty;

    public InterviewOutcome Outcome { get; set; } = InterviewOutcome.Pending;

    [StringLength(2000)]
    public string? Feedback { get; set; }

    [JsonIgnore]
    public DateTime EndsAt => ScheduledAt.AddMinutes(DurationMinutes);

    [JsonIgnore]
    public bool IsUpcoming => Outcome == InterviewOutcome.Pending && ScheduledAt >= DateTime.Now;
}
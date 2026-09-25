using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TalentCRM.Models;

// Represents one candidate applying for one job.
public class JobApplication : Entity
{
    public int CandidateId { get; set; }
    public int JobId { get; set; }

    public ApplicationStage Stage { get; set; } = ApplicationStage.Applied;

    // Tracks when the stage last changed.
    public DateTime StageChangedAt { get; set; } = DateTime.Now;

    [StringLength(1000)]
    public string? Notes { get; set; }

    [JsonIgnore]
    public bool IsActive => !Stage.IsFinal();
}
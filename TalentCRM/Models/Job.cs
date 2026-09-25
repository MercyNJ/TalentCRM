using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TalentCRM.Models;

// Represents a vacancy that a client wants filled.
public class Job : Entity
{
    [Required, StringLength(100)]
    [Display(Name = "Job title")]
    public string Title { get; set; } = string.Empty;

    // Links the job to its client by Id.
    [Range(1, int.MaxValue, ErrorMessage = "Choose a client.")]
    [Display(Name = "Client")]
    public int ClientId { get; set; }

    [StringLength(80)]
    public string? Location { get; set; }

    [Display(Name = "Employment type")]
    public EmploymentType EmploymentType { get; set; } = EmploymentType.FullTime;

    // decimal? makes the salary optional.
    [Range(0, 100_000_000)]
    [Display(Name = "Salary from (KES / month)")]
    public decimal? SalaryMin { get; set; }

    [Range(0, 100_000_000)]
    [Display(Name = "Salary to (KES / month)")]
    public decimal? SalaryMax { get; set; }

    [Range(1, 100)]
    public int Openings { get; set; } = 1;

    public JobStatus Status { get; set; } = JobStatus.Open;

    [StringLength(2000)]
    public string? Description { get; set; }

    [StringLength(300)]
    [Display(Name = "Required skills (comma separated)")]
    public string? RequiredSkills { get; set; }

    // Uses tuple pattern matching with a switch expression.
    [JsonIgnore]
    public string SalaryText => (SalaryMin, SalaryMax) switch
    {
        (null, null) => "Not specified",
        (decimal min, null) => $"From KES {min:N0}",
        (null, decimal max) => $"Up to KES {max:N0}",
        (decimal min, decimal max) => $"KES {min:N0} – {max:N0}"
    };

    [JsonIgnore]
    public IReadOnlyList<string> RequiredSkillList =>
        (RequiredSkills ?? string.Empty).Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
}
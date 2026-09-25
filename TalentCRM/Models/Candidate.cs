using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TalentCRM.Models;

// Represents a person looking for work.
public class Candidate : Entity
{
    [Required, StringLength(50)]
    [Display(Name = "First name")]
    public string FirstName { get; set; } = string.Empty;

    [Required, StringLength(50)]
    [Display(Name = "Last name")]
    public string LastName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(120)]
    public string Email { get; set; } = string.Empty;

    // Nullable reference type.
    [Phone, StringLength(30)]
    public string? Phone { get; set; }

    [StringLength(80)]
    public string? Location { get; set; }

    [Range(0, 50)]
    [Display(Name = "Years of experience")]
    public int YearsOfExperience { get; set; }

    [StringLength(300)]
    [Display(Name = "Skills (comma separated)")]
    public string? Skills { get; set; }

    public CandidateStatus Status { get; set; } = CandidateStatus.Available;

    // Computed properties are not saved to JSON.
    [JsonIgnore]
    public string FullName => $"{FirstName} {LastName}";

    [JsonIgnore]
    public IReadOnlyList<string> SkillList =>
        (Skills ?? string.Empty).Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

    // Uses LINQ Any() for a case-insensitive skill check.
    public bool HasSkill(string skill) =>
        SkillList.Any(s => s.Equals(skill.Trim(), StringComparison.OrdinalIgnoreCase));
}
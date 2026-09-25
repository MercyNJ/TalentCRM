using System.ComponentModel.DataAnnotations;

namespace TalentCRM.Models;

// Represents a company that hires through the system.
public class Client : Entity
{
    [Required, StringLength(100)]
    [Display(Name = "Company name")]
    public string CompanyName { get; set; } = string.Empty;

    [StringLength(60)]
    public string? Industry { get; set; }

    [Required, StringLength(80)]
    [Display(Name = "Contact person")]
    public string ContactName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(120)]
    [Display(Name = "Contact email")]
    public string ContactEmail { get; set; } = string.Empty;

    [Phone, StringLength(30)]
    [Display(Name = "Contact phone")]
    public string? ContactPhone { get; set; }

    [StringLength(80)]
    public string? Location { get; set; }
}
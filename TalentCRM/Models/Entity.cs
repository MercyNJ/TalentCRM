namespace TalentCRM.Models;

// Base class shared by the main domain models.
public abstract class Entity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
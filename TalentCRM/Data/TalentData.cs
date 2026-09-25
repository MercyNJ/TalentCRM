using TalentCRM.Models;

namespace TalentCRM.Data;

// In-memory application data persisted to JSON.
public class TalentData
{
    public List<Candidate> Candidates { get; set; } = [];
    public List<Client> Clients { get; set; } = [];
    public List<Job> Jobs { get; set; } = [];
    public List<JobApplication> Applications { get; set; } = [];
    public List<Interview> Interviews { get; set; } = [];
}
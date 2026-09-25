using TalentCRM.Data;
using TalentCRM.Models;

namespace TalentCRM.Services;

public class CandidateService
{
    private readonly ITalentStore _store;

    public CandidateService(ITalentStore store) => _store = store;

    public List<CandidateRow> Search(string? search, CandidateStatus? status) =>
        _store.Read(data =>
        {
            IEnumerable<Candidate> query = data.Candidates;

            if (!string.IsNullOrWhiteSpace(search))
            {
                string s = search.Trim();
                query = query.Where(c => c.FullName.ContainsText(s)
                                      || c.Email.ContainsText(s)
                                      || c.Location.ContainsText(s)
                                      || c.HasSkill(s));
            }

            if (status is not null)
            {
                query = query.Where(c => c.Status == status);
            }

            return query
                .OrderBy(c => c.LastName).ThenBy(c => c.FirstName)
                .Select(c => new CandidateRow(
                    c,
                    data.Applications.Count(a => a.CandidateId == c.Id && a.IsActive)))
                .ToList();
        });

    public Candidate? Find(int id) =>
        _store.Read(data => data.Candidates.FirstOrDefault(c => c.Id == id));

    public (Candidate Candidate, List<ApplicationRow> Applications)? GetProfile(int id) =>
        _store.Read<(Candidate, List<ApplicationRow>)?>(data =>
        {
            Candidate? candidate = data.Candidates.FirstOrDefault(c => c.Id == id);
            if (candidate is null) return null;

            var applications = data.Applications.Where(a => a.CandidateId == id).ToRows(data);
            return (candidate, applications);
        });

    public Task<Candidate> AddAsync(Candidate candidate) =>
        _store.WriteAsync(data =>
        {
            EnsureEmailIsUnique(data, candidate.Email, ignoreId: 0);

            candidate.Id = data.Candidates.NextId();
            candidate.Email = candidate.Email.Trim();
            candidate.CreatedAt = DateTime.Now;
            data.Candidates.Add(candidate);
            return candidate;
        });

    public Task UpdateAsync(Candidate changes) =>
        _store.WriteAsync(data =>
        {
            Candidate existing = data.Candidates.GetOrThrow(changes.Id);
            EnsureEmailIsUnique(data, changes.Email, ignoreId: changes.Id);

            // Copy only the fields the form is allowed to change (Id and CreatedAt stay as they were).
            existing.FirstName = changes.FirstName;
            existing.LastName = changes.LastName;
            existing.Email = changes.Email.Trim();
            existing.Phone = changes.Phone;
            existing.Location = changes.Location;
            existing.YearsOfExperience = changes.YearsOfExperience;
            existing.Skills = changes.Skills;
            existing.Status = changes.Status;
        });

    public Task DeleteAsync(int id) =>
        _store.WriteAsync(data =>
        {
            Candidate candidate = data.Candidates.GetOrThrow(id);

            int active = data.Applications.Count(a => a.CandidateId == id && a.IsActive);
            if (active > 0)
            {
                throw new BusinessRuleException(
                    $"{candidate.FullName} still has {active} active application(s). Withdraw or reject them first.");
            }

            // Remove the candidate's old (finished) applications and their interviews too.
            HashSet<int> applicationIds = data.Applications
                .Where(a => a.CandidateId == id)
                .Select(a => a.Id)
                .ToHashSet();

            data.Interviews.RemoveAll(i => applicationIds.Contains(i.ApplicationId));
            data.Applications.RemoveAll(a => applicationIds.Contains(a.Id));
            data.Candidates.Remove(candidate);
        });

    private static void EnsureEmailIsUnique(TalentData data, string email, int ignoreId)
    {
        bool taken = data.Candidates.Any(c =>
            c.Id != ignoreId && c.Email.Equals(email.Trim(), StringComparison.OrdinalIgnoreCase));

        if (taken)
        {
            throw new BusinessRuleException($"Another candidate already uses the email {email}.");
        }
    }
}

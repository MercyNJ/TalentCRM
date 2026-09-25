using TalentCRM.Models;

namespace TalentCRM.Data;

// Sample data used when the JSON file doesn't exist.
public static class SeedData
{
    public static TalentData Create()
    {
        DateTime today = DateTime.Today;

        List<Client> clients =
        [
            new Client { Id = 1, CompanyName = "Savanna Pay", Industry = "Fintech", ContactName = "Grace Wanjiru",
                         ContactEmail = "grace.wanjiru@savannapay.example.com", ContactPhone = "+254 700 111 222",
                         Location = "Nairobi", CreatedAt = today.AddDays(-60) },
            new Client { Id = 2, CompanyName = "Jacaranda Logistics", Industry = "Logistics", ContactName = "Peter Otieno",
                         ContactEmail = "p.otieno@jacaranda.example.com", ContactPhone = "+254 711 333 444",
                         Location = "Mombasa", CreatedAt = today.AddDays(-45) },
            new Client { Id = 3, CompanyName = "Kilimo Fresh Co.", Industry = "AgriTech", ContactName = "Amina Hassan",
                         ContactEmail = "amina@kilimofresh.example.com", Location = "Nakuru", CreatedAt = today.AddDays(-30) },
            new Client { Id = 4, CompanyName = "Pwani Care Clinics", Industry = "Healthcare", ContactName = "Dr. James Mwangi",
                         ContactEmail = "jmwangi@pwanicare.example.com", Location = "Kilifi", CreatedAt = today.AddDays(-10) },
        ];

        List<Job> jobs =
        [
            new Job { Id = 1, ClientId = 1, Title = "Senior .NET Developer", Location = "Nairobi (hybrid)",
                      EmploymentType = EmploymentType.FullTime, SalaryMin = 350_000, SalaryMax = 500_000, Openings = 2,
                      RequiredSkills = "C#, ASP.NET Core, SQL", CreatedAt = today.AddDays(-40),
                      Description = "Build and maintain payment APIs used by thousands of merchants." },
            new Job { Id = 2, ClientId = 1, Title = "QA Engineer", Location = "Nairobi",
                      EmploymentType = EmploymentType.Contract, SalaryMin = 180_000, Openings = 1,
                      RequiredSkills = "Testing, Selenium, C#", CreatedAt = today.AddDays(-25) },
            new Job { Id = 3, ClientId = 2, Title = "Operations Analyst", Location = "Mombasa",
                      EmploymentType = EmploymentType.FullTime, SalaryMin = 120_000, SalaryMax = 160_000, Openings = 1,
                      RequiredSkills = "Excel, SQL, Power BI", Status = JobStatus.Filled, CreatedAt = today.AddDays(-44) },
            new Job { Id = 4, ClientId = 3, Title = "Full-Stack Developer", Location = "Remote",
                      EmploymentType = EmploymentType.FullTime, SalaryMin = 250_000, SalaryMax = 380_000, Openings = 1,
                      RequiredSkills = "C#, React, SQL", CreatedAt = today.AddDays(-20) },
            new Job { Id = 5, ClientId = 3, Title = "Data Science Intern", Location = "Nakuru",
                      EmploymentType = EmploymentType.Internship, SalaryMax = 40_000, Openings = 2,
                      RequiredSkills = "Python, Statistics", CreatedAt = today.AddDays(-5) },
            new Job { Id = 6, ClientId = 4, Title = "IT Support Officer", Location = "Kilifi",
                      EmploymentType = EmploymentType.FullTime, SalaryMin = 70_000, SalaryMax = 90_000, Openings = 1,
                      RequiredSkills = "Networking, Windows, Customer service", CreatedAt = today.AddDays(-3) },
        ];

        List<Candidate> candidates =
        [
            new Candidate { Id = 1, FirstName = "Brian", LastName = "Kamau", Email = "brian.kamau@example.com",
                            Phone = "+254 722 000 001", Location = "Nairobi", YearsOfExperience = 6,
                            Skills = "C#, ASP.NET Core, SQL, Azure", CreatedAt = today.AddDays(-38) },
            new Candidate { Id = 2, FirstName = "Faith", LastName = "Achieng", Email = "faith.achieng@example.com",
                            Location = "Kisumu", YearsOfExperience = 4, Skills = "C#, React, SQL, TypeScript",
                            CreatedAt = today.AddDays(-35) },
            new Candidate { Id = 3, FirstName = "Kevin", LastName = "Mutua", Email = "kevin.mutua@example.com",
                            Location = "Machakos", YearsOfExperience = 2, Skills = "Testing, Selenium, C#",
                            CreatedAt = today.AddDays(-22) },
            new Candidate { Id = 4, FirstName = "Mercy", LastName = "Chebet", Email = "mercy.chebet@example.com",
                            Location = "Mombasa", YearsOfExperience = 3, Skills = "Excel, SQL, Power BI",
                            Status = CandidateStatus.Placed, CreatedAt = today.AddDays(-43) },
            new Candidate { Id = 5, FirstName = "Daniel", LastName = "Kiprono", Email = "daniel.kiprono@example.com",
                            Location = "Eldoret", YearsOfExperience = 8, Skills = "C#, ASP.NET Core, Azure, Leadership",
                            CreatedAt = today.AddDays(-30) },
            new Candidate { Id = 6, FirstName = "Wanjiku", LastName = "Njoroge", Email = "wanjiku.njoroge@example.com",
                            Location = "Nakuru", YearsOfExperience = 0, Skills = "Python, Statistics, Excel",
                            CreatedAt = today.AddDays(-4) },
            new Candidate { Id = 7, FirstName = "Hassan", LastName = "Omar", Email = "hassan.omar@example.com",
                            Location = "Kilifi", YearsOfExperience = 5, Skills = "Networking, Windows, Linux",
                            CreatedAt = today.AddDays(-2) },
            new Candidate { Id = 8, FirstName = "Joy", LastName = "Wambui", Email = "joy.wambui@example.com",
                            Location = "Nairobi", YearsOfExperience = 1, Skills = "React, JavaScript",
                            Status = CandidateStatus.NotLooking, CreatedAt = today.AddDays(-15) },
        ];

        List<JobApplication> applications =
        [
            new JobApplication { Id = 1, CandidateId = 1, JobId = 1, Stage = ApplicationStage.Interviewing,
                                 CreatedAt = today.AddDays(-36), StageChangedAt = today.AddDays(-7),
                                 Notes = "Strong API background. Client keen to move fast." },
            new JobApplication { Id = 2, CandidateId = 5, JobId = 1, Stage = ApplicationStage.Offered,
                                 CreatedAt = today.AddDays(-28), StageChangedAt = today.AddDays(-1),
                                 Notes = "Offer sent at KES 480k. Waiting for his reply." },
            new JobApplication { Id = 3, CandidateId = 2, JobId = 1, Stage = ApplicationStage.Screening,
                                 CreatedAt = today.AddDays(-10), StageChangedAt = today.AddDays(-9) },
            new JobApplication { Id = 4, CandidateId = 3, JobId = 2, Stage = ApplicationStage.Interviewing,
                                 CreatedAt = today.AddDays(-20), StageChangedAt = today.AddDays(-6) },
            new JobApplication { Id = 5, CandidateId = 4, JobId = 3, Stage = ApplicationStage.Hired,
                                 CreatedAt = today.AddDays(-42), StageChangedAt = today.AddDays(-12),
                                 Notes = "Started on the 1st." },
            new JobApplication { Id = 6, CandidateId = 2, JobId = 4, Stage = ApplicationStage.Interviewing,
                                 CreatedAt = today.AddDays(-18), StageChangedAt = today.AddDays(-4) },
            new JobApplication { Id = 7, CandidateId = 1, JobId = 4, Stage = ApplicationStage.Withdrawn,
                                 CreatedAt = today.AddDays(-17), StageChangedAt = today.AddDays(-14),
                                 Notes = "Prefers the Savanna Pay role." },
            new JobApplication { Id = 8, CandidateId = 6, JobId = 5, Stage = ApplicationStage.Applied,
                                 CreatedAt = today.AddDays(-3), StageChangedAt = today.AddDays(-3) },
            new JobApplication { Id = 9, CandidateId = 8, JobId = 4, Stage = ApplicationStage.Rejected,
                                 CreatedAt = today.AddDays(-15), StageChangedAt = today.AddDays(-11),
                                 Notes = "Needs more backend experience." },
        ];

        List<Interview> interviews =
        [
            // Past interviews
            new Interview { Id = 1, ApplicationId = 1, ScheduledAt = today.AddDays(-5).AddHours(10), Type = InterviewType.Video,
                            Interviewer = "Grace Wanjiru", Outcome = InterviewOutcome.Passed,
                            Feedback = "Clear communicator. Good grasp of async code.", CreatedAt = today.AddDays(-7) },
            new Interview { Id = 2, ApplicationId = 2, ScheduledAt = today.AddDays(-8).AddHours(14), Type = InterviewType.Technical,
                            DurationMinutes = 90, Interviewer = "Savanna Pay tech lead", Outcome = InterviewOutcome.Passed,
                            CreatedAt = today.AddDays(-12) },
            new Interview { Id = 3, ApplicationId = 5, ScheduledAt = today.AddDays(-20).AddHours(11), Type = InterviewType.InPerson,
                            Interviewer = "Peter Otieno", Outcome = InterviewOutcome.Passed, CreatedAt = today.AddDays(-25) },

            // Upcoming interviews
            new Interview { Id = 4, ApplicationId = 1, ScheduledAt = today.AddDays(1).AddHours(10), Type = InterviewType.Technical,
                            DurationMinutes = 90, Interviewer = "Savanna Pay tech lead", CreatedAt = today.AddDays(-4) },
            new Interview { Id = 5, ApplicationId = 4, ScheduledAt = today.AddDays(2).AddHours(15), Type = InterviewType.Video,
                            Interviewer = "Grace Wanjiru", CreatedAt = today.AddDays(-6) },
            new Interview { Id = 6, ApplicationId = 6, ScheduledAt = today.AddDays(4).AddHours(9).AddMinutes(30),
                            Type = InterviewType.InPerson, Interviewer = "Amina Hassan", CreatedAt = today.AddDays(-4) },
        ];

        return new TalentData
        {
            Clients = clients,
            Jobs = jobs,
            Candidates = candidates,
            Applications = applications,
            Interviews = interviews
        };
    }
}
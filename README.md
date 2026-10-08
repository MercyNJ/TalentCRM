# TalentCRM

TalentCRM is a recruitment and talent management web application for managing **candidates, clients, jobs, applications, and interviews** throughout the hiring process.

## Features

* Candidate management
* Client management
* Job vacancy management
* Application tracking
* Interview scheduling
* Recruitment dashboard
* Application pipeline and business rules

## Tech Stack

* C#
* ASP.NET Core Razor Pages
* .NET 8
* HTML / Razor
* CSS
* JSON file storage

## Application Flow

```text
Client → Job → Candidate → Application → Interview → Hired
```

## Getting Started

### Visual Studio 2022

1. Open `TalentCRM.sln`
2. Press **F5**
3. The application will open in your browser

### Command Line

```bash
dotnet run --project TalentCRM
```

Sample data is created automatically on the first run.

## Example Workflow

A typical recruitment workflow moves from client and job creation through application, screening, interviewing, and hiring.

1. **Create a client** and add a job opening.
2. **Add candidates** and apply them to the job.
3. **Screen candidates** and schedule interviews.
4. **Progress an application** through `Applied → Screening → Interviewing → Offered → Hired`.
5. **Complete the hire** and verify that the job, candidate, remaining applications, and dashboard are updated automatically.

The application also validates key business rules, including duplicate candidates and applications, invalid salary ranges, past interviews, closed jobs, and protected deletes.

## Business Rules

- **Candidates:** Email addresses must be unique. Candidates with active applications cannot be deleted, and candidates marked **Not looking** cannot apply.
- **Clients:** Company names must be unique. Clients with jobs cannot be deleted.
- **Jobs:** Salary ranges must be valid, openings cannot be reduced below the number already hired, and only **Open** jobs accept applications.
- **Applications:** A candidate can have only one active application per job. Applications progress through `Applied → Screening → Interviewing → Offered → Hired`, with **Rejected** or **Withdrawn** available where appropriate.
- **Interviews:** Interviews can only be scheduled during the appropriate application stages, must be in the future, and cannot overlap for the same candidate.
- **Hiring:** When the final opening is filled, the job becomes **Filled** and remaining applications are closed.
- **Interview outcomes:** Passed, Failed, or No-show outcomes can only be recorded after the scheduled interview time.

## Project Structure

```text
TalentCRM/
├── Models/       # Application entities
├── Data/         # Data storage
├── Services/     # Business logic
├── Pages/        # Razor Pages
└── wwwroot/      # CSS and static files
```
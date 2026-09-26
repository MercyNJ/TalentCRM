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

## Project Structure

```text
TalentCRM/
├── Models/       # Application entities
├── Data/         # Data storage
├── Services/     # Business logic
├── Pages/        # Razor Pages
└── wwwroot/      # CSS and static files
```
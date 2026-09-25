namespace TalentCRM.Models;

// Fixed set of named values.
public enum CandidateStatus
{
    Available,
    Placed,
    NotLooking
}

public enum EmploymentType
{
    FullTime,
    PartTime,
    Contract,
    Internship
}

public enum JobStatus
{
    Open,
    OnHold,
    Filled,
    Closed
}

// Recruitment pipeline stages.
public enum ApplicationStage
{
    Applied,
    Screening,
    Interviewing,
    Offered,
    Hired,
    Rejected,
    Withdrawn
}

public enum InterviewType
{
    Phone,
    Video,
    InPerson,
    Technical
}

public enum InterviewOutcome
{
    Pending,
    Passed,
    Failed,
    NoShow,
    Cancelled
}
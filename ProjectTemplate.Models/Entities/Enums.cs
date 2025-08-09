namespace ProjectTemplate.Models.Entities;

public enum RequestType
{
    Leave = 1,
    Expense = 2,
    Training = 3,
    ITSupport = 4,
    ProfileUpdate = 5
}

public enum RequestStatus
{
    Pending = 1,
    Approved = 2,
    Rejected = 3,
    Archived = 4
}

public enum StepStatus
{
    Pending = 1,
    Approved = 2,
    Rejected = 3
}

public enum LeaveType
{
    AnnualLeave = 0,
    SickLeave = 1,
    PersonalLeave = 2,
    MaternityLeave = 3,
    PaternityLeave = 4,
    EmergencyLeave = 5,
    StudyLeave = 6,
    UnpaidLeave = 7
}

public enum LeaveStatus
{
    Pending = 0,
    Approved = 1,
    Rejected = 2,
    Cancelled = 3
}

public enum ReviewType
{
    Annual = 0,
    Quarterly = 1,
    Probationary = 2,
    Project = 3,
    ThreeSixty = 4,
    MidYear = 5
}

public enum ReviewStatus
{
    Draft = 0,
    InProgress = 1,
    Completed = 2,
    Overdue = 3,
    Cancelled = 4
}

public enum ReviewPriority
{
    Low = 0,
    Medium = 1,
    High = 2,
    Critical = 3
}

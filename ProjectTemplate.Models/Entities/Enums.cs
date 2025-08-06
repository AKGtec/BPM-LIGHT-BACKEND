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

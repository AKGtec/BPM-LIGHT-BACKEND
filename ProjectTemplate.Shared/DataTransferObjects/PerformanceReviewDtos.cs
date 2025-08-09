using ProjectTemplate.Models.Entities;
using ProjectTemplate.Shared.RequestFeatures;

namespace ProjectTemplate.Shared.DataTransferObjects;

public class PerformanceReviewDto : BaseDto
{
    public string Title { get; set; } = string.Empty;
    public string EmployeeId { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public string EmployeePosition { get; set; } = string.Empty;
    public ReviewType ReviewType { get; set; }
    public ReviewStatus Status { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public int Progress { get; set; }
    public ReviewPriority Priority { get; set; }
    public string? Description { get; set; }
    public List<string> Goals { get; set; } = new List<string>();
    public string? Feedback { get; set; }
    public decimal? Rating { get; set; }
    public string? ReviewerId { get; set; }
    public string? ReviewerName { get; set; }
    public DateTime? CompletedDate { get; set; }
}

public class CreatePerformanceReviewDto
{
    public string Title { get; set; } = string.Empty;
    public string EmployeeId { get; set; } = string.Empty;
    public ReviewType ReviewType { get; set; }
    public DateTime DueDate { get; set; }
    public ReviewPriority Priority { get; set; }
    public string? Description { get; set; }
    public List<string>? Goals { get; set; }
}

public class UpdatePerformanceReviewDto
{
    public string? Title { get; set; }
    public ReviewType? ReviewType { get; set; }
    public DateTime? DueDate { get; set; }
    public ReviewPriority? Priority { get; set; }
    public string? Description { get; set; }
    public List<string>? Goals { get; set; }
    public int? Progress { get; set; }
    public string? Feedback { get; set; }
    public decimal? Rating { get; set; }
}

public class CompleteReviewDto
{
    public string Feedback { get; set; } = string.Empty;
    public decimal Rating { get; set; }
}

public class CancelReviewDto
{
    public string? Reason { get; set; }
}

public class BulkUpdateStatusDto
{
    public List<Guid> Ids { get; set; } = new List<Guid>();
    public ReviewStatus Status { get; set; }
}

public class BulkDeleteDto
{
    public List<Guid> Ids { get; set; } = new List<Guid>();
}

public class PerformanceReviewStatisticsDto
{
    public int TotalReviews { get; set; }
    public int PendingReviews { get; set; }
    public int CompletedReviews { get; set; }
    public int OverdueReviews { get; set; }
    public decimal AverageRating { get; set; }
    public decimal CompletionRate { get; set; }
}

public class ReviewTypeOptionDto
{
    public ReviewType Value { get; set; }
    public string Label { get; set; } = string.Empty;
}

public class ReviewStatusOptionDto
{
    public ReviewStatus Value { get; set; }
    public string Label { get; set; } = string.Empty;
}

public class ReviewPriorityOptionDto
{
    public ReviewPriority Value { get; set; }
    public string Label { get; set; } = string.Empty;
}

public class PerformanceReviewParameters : RequestParameters
{
    public string? SearchTerm { get; set; }
    public ReviewStatus? Status { get; set; }
    public ReviewType? Type { get; set; }
    public ReviewPriority? Priority { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? EmployeeId { get; set; }
    public string? ReviewerId { get; set; }
}
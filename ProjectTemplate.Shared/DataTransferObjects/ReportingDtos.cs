using ProjectTemplate.Models.Entities;

namespace ProjectTemplate.Shared.DataTransferObjects;

public class ReportFilterDto
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? RequestType { get; set; }
    public string? Status { get; set; }
    public string? Department { get; set; }
    public string? UserId { get; set; }
    public string? WorkflowId { get; set; }
}

public class ReportDataDto
{
    public int TotalRequests { get; set; }
    public int PendingRequests { get; set; }
    public int ApprovedRequests { get; set; }
    public int RejectedRequests { get; set; }
    public double AverageProcessingTime { get; set; }
    public Dictionary<string, int> RequestsByType { get; set; } = new();
    public Dictionary<string, int> RequestsByStatus { get; set; } = new();
    public Dictionary<string, int> RequestsByDepartment { get; set; } = new();
    public Dictionary<string, double> ProcessingTimeByType { get; set; } = new();
    public Dictionary<string, double> ApprovalRateByManager { get; set; } = new();
    public List<MonthlyTrendDto> MonthlyTrends { get; set; } = new();
}

public class MonthlyTrendDto
{
    public string Month { get; set; } = string.Empty;
    public int Submitted { get; set; }
    public int Approved { get; set; }
    public int Rejected { get; set; }
}

public class UserActivityReportDto
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public int TotalRequests { get; set; }
    public int PendingRequests { get; set; }
    public int ApprovedRequests { get; set; }
    public int RejectedRequests { get; set; }
    public double AverageResponseTime { get; set; }
    public DateTime LastActivity { get; set; }
}

public class WorkflowPerformanceReportDto
{
    public string WorkflowId { get; set; } = string.Empty;
    public string WorkflowName { get; set; } = string.Empty;
    public int TotalRequests { get; set; }
    public double AverageCompletionTime { get; set; }
    public List<BottleneckStepDto> BottleneckSteps { get; set; } = new();
    public double CompletionRate { get; set; }
    public double? UserSatisfactionScore { get; set; }
}

public class BottleneckStepDto
{
    public string StepName { get; set; } = string.Empty;
    public double AverageTime { get; set; }
    public int PendingCount { get; set; }
    public double ImpactScore { get; set; }
}

public class SystemHealthReportDto
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int TotalWorkflows { get; set; }
    public int ActiveWorkflows { get; set; }
    public double SystemUptime { get; set; }
    public double AverageResponseTime { get; set; }
    public double ErrorRate { get; set; }
    public StorageUsageDto StorageUsage { get; set; } = new();
    public DatabaseHealthDto DatabaseHealth { get; set; } = new();
}

public class StorageUsageDto
{
    public long Used { get; set; }
    public long Total { get; set; }
    public double Percentage { get; set; }
}

public class DatabaseHealthDto
{
    public int ConnectionCount { get; set; }
    public double QueryPerformance { get; set; }
    public double IndexHealth { get; set; }
}

public class ExportOptionsDto
{
    public string Format { get; set; } = "excel"; // pdf, excel, csv
    public bool IncludeCharts { get; set; } = true;
    public bool IncludeRawData { get; set; } = false;
    public string? Template { get; set; }
}

public class RequestTrendsDto
{
    public List<string> Labels { get; set; } = new();
    public List<DatasetDto> Datasets { get; set; } = new();
}

public class DatasetDto
{
    public string Label { get; set; } = string.Empty;
    public List<int> Data { get; set; } = new();
    public string? BackgroundColor { get; set; }
    public string? BorderColor { get; set; }
}

public class ApprovalRatesDto
{
    public double Overall { get; set; }
    public Dictionary<string, double> ByType { get; set; } = new();
    public Dictionary<string, double> ByDepartment { get; set; } = new();
    public Dictionary<string, double> ByManager { get; set; } = new();
}

public class ProcessingTimeAnalysisDto
{
    public double Average { get; set; }
    public double Median { get; set; }
    public Dictionary<string, ProcessingTimeStatsDto> ByType { get; set; } = new();
    public Dictionary<string, ProcessingTimeStatsDto> ByStep { get; set; } = new();
    public List<DistributionDto> Distribution { get; set; } = new();
}

public class ProcessingTimeStatsDto
{
    public double Average { get; set; }
    public double Median { get; set; }
}

public class DistributionDto
{
    public string Range { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
}

public class BottleneckAnalysisDto
{
    public string WorkflowId { get; set; } = string.Empty;
    public string WorkflowName { get; set; } = string.Empty;
    public List<BottleneckStepDto> Bottlenecks { get; set; } = new();
}

public class DepartmentReportDto
{
    public string DepartmentName { get; set; } = string.Empty;
    public int TotalEmployees { get; set; }
    public int TotalRequests { get; set; }
    public double AverageProcessingTime { get; set; }
    public List<RequestTypeCountDto> TopRequestTypes { get; set; } = new();
    public List<UserActivityReportDto> EmployeeActivity { get; set; } = new();
    public List<object> Trends { get; set; } = new();
}

public class RequestTypeCountDto
{
    public string Type { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class ManagerReportDto
{
    public string ManagerName { get; set; } = string.Empty;
    public int TeamSize { get; set; }
    public int PendingApprovals { get; set; }
    public double AverageApprovalTime { get; set; }
    public double ApprovalRate { get; set; }
    public List<UserActivityReportDto> TeamActivity { get; set; } = new();
    public List<object> WorkloadDistribution { get; set; } = new();
}

public class CustomReportConfigDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ReportFilterDto Filters { get; set; } = new();
    public List<string> Metrics { get; set; } = new();
    public List<string> ChartTypes { get; set; } = new();
    public ScheduleDto? Schedule { get; set; }
}

public class ScheduleDto
{
    public string Frequency { get; set; } = string.Empty; // daily, weekly, monthly
    public List<string> Recipients { get; set; } = new();
}

public class CustomReportDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastRun { get; set; }
    public bool IsScheduled { get; set; }
}

public class ScheduledReportDto
{
    public string Id { get; set; } = string.Empty;
    public string ReportName { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
    public DateTime NextRun { get; set; }
    public List<string> Recipients { get; set; } = new();
    public bool IsActive { get; set; }
}

public class ReportScheduleDto
{
    public string Frequency { get; set; } = string.Empty; // daily, weekly, monthly
    public List<string> Recipients { get; set; } = new();
    public string Format { get; set; } = "pdf"; // pdf, excel
}
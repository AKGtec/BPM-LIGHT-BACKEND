using ProjectTemplate.Models.Entities;
using ProjectTemplate.Shared.DataTransferObjects;
using ProjectTemplate.Shared.RequestFeatures;

namespace ProjectTemplate.Service.Contracts;

public interface IPerformanceReviewService
{
    // Performance Reviews CRUD
    Task<PaginatedResponse<PerformanceReviewDto>> GetPerformanceReviewsAsync(PerformanceReviewParameters parameters, bool trackChanges);
    Task<PaginatedResponse<PerformanceReviewDto>> GetMyPerformanceReviewsAsync(string employeeId, PerformanceReviewParameters parameters, bool trackChanges);
    Task<PaginatedResponse<PerformanceReviewDto>> GetPendingPerformanceReviewsAsync(PerformanceReviewParameters parameters, bool trackChanges);
    Task<PerformanceReviewDto?> GetPerformanceReviewByIdAsync(Guid id, bool trackChanges);
    Task<PerformanceReviewDto> CreatePerformanceReviewAsync(CreatePerformanceReviewDto createDto, string currentUserId);
    Task<PerformanceReviewDto> UpdatePerformanceReviewAsync(Guid id, UpdatePerformanceReviewDto updateDto, bool trackChanges);
    Task DeletePerformanceReviewAsync(Guid id, bool trackChanges);

    // Review Actions
    Task StartReviewAsync(Guid id, string currentUserId);
    Task CompleteReviewAsync(Guid id, CompleteReviewDto completeDto, string currentUserId);
    Task CancelReviewAsync(Guid id, CancelReviewDto cancelDto, string currentUserId);

    // Bulk Operations
    Task BulkUpdateStatusAsync(BulkUpdateStatusDto bulkUpdateDto, string currentUserId);
    Task BulkDeleteAsync(BulkDeleteDto bulkDeleteDto, string currentUserId);

    // Statistics and Reports
    Task<PerformanceReviewStatisticsDto> GetPerformanceReviewStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<byte[]> ExportPerformanceReviewsAsync(string format, DateTime? startDate = null, DateTime? endDate = null);

    // Utility methods
    string GetReviewTypeLabel(ReviewType type);
    string GetReviewStatusLabel(ReviewStatus status);
    string GetReviewPriorityLabel(ReviewPriority priority);
    IEnumerable<ReviewTypeOptionDto> GetReviewTypeOptions();
    IEnumerable<ReviewStatusOptionDto> GetReviewStatusOptions();
    IEnumerable<ReviewPriorityOptionDto> GetReviewPriorityOptions();
}
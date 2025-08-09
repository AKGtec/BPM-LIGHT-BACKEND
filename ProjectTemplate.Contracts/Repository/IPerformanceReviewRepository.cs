using ProjectTemplate.Models.Entities;
using ProjectTemplate.Shared.RequestFeatures;
using ProjectTemplate.Shared.DataTransferObjects;

namespace ProjectTemplate.Contracts.Repository;

public interface IPerformanceReviewRepository : IRepositoryBase<PerformanceReview>
{
    Task<PagedList<PerformanceReview>> GetAllPerformanceReviewsAsync(PerformanceReviewParameters parameters, bool trackChanges);
    Task<PagedList<PerformanceReview>> GetPerformanceReviewsByEmployeeIdAsync(string employeeId, PerformanceReviewParameters parameters, bool trackChanges);
    Task<PagedList<PerformanceReview>> GetPendingPerformanceReviewsAsync(PerformanceReviewParameters parameters, bool trackChanges);
    Task<PerformanceReview?> GetPerformanceReviewByIdAsync(Guid id, bool trackChanges);
    Task<IEnumerable<PerformanceReview>> GetPerformanceReviewsByStatusAsync(ReviewStatus status, bool trackChanges);
    Task<IEnumerable<PerformanceReview>> GetOverduePerformanceReviewsAsync(bool trackChanges);
    Task<IEnumerable<PerformanceReview>> GetPerformanceReviewsByDateRangeAsync(DateTime startDate, DateTime endDate, bool trackChanges);
    void CreatePerformanceReview(PerformanceReview performanceReview);
    void UpdatePerformanceReview(PerformanceReview performanceReview);
    void DeletePerformanceReview(PerformanceReview performanceReview);
}
using Microsoft.EntityFrameworkCore;
using ProjectTemplate.Contracts.Repository;
using ProjectTemplate.Models.DataSource;
using ProjectTemplate.Models.Entities;
using ProjectTemplate.Shared.RequestFeatures;
using ProjectTemplate.Shared.DataTransferObjects;

namespace ProjectTemplate.Repository.Repository;

public sealed class PerformanceReviewRepository : RepositoryBase<PerformanceReview>, IPerformanceReviewRepository
{
    public PerformanceReviewRepository(ProjectTemplateContext repositoryContext) : base(repositoryContext)
    {
    }

    public async Task<PagedList<PerformanceReview>> GetAllPerformanceReviewsAsync(PerformanceReviewParameters parameters, bool trackChanges)
    {
        var query = FindAll(trackChanges)
            .Include(pr => pr.Employee)
            .Include(pr => pr.Reviewer)
            .AsQueryable();

        // Apply filters
        query = ApplyFilters(query, parameters);

        // Apply sorting
        query = ApplySorting(query, parameters.SortBy, parameters.SortDirection);

        return await PagedList<PerformanceReview>.ToPagedListAsync(query, parameters.PageNumber, parameters.PageSize);
    }

    public async Task<PagedList<PerformanceReview>> GetPerformanceReviewsByEmployeeIdAsync(string employeeId, PerformanceReviewParameters parameters, bool trackChanges)
    {
        var query = FindByCondition(pr => pr.EmployeeId == employeeId, trackChanges)
            .Include(pr => pr.Employee)
            .Include(pr => pr.Reviewer)
            .AsQueryable();

        // Apply filters
        query = ApplyFilters(query, parameters);

        // Apply sorting
        query = ApplySorting(query, parameters.SortBy, parameters.SortDirection);

        return await PagedList<PerformanceReview>.ToPagedListAsync(query, parameters.PageNumber, parameters.PageSize);
    }

    public async Task<PagedList<PerformanceReview>> GetPendingPerformanceReviewsAsync(PerformanceReviewParameters parameters, bool trackChanges)
    {
        var query = FindByCondition(pr => pr.Status == ReviewStatus.Draft || pr.Status == ReviewStatus.InProgress, trackChanges)
            .Include(pr => pr.Employee)
            .Include(pr => pr.Reviewer)
            .AsQueryable();

        // Apply filters
        query = ApplyFilters(query, parameters);

        // Apply sorting
        query = ApplySorting(query, parameters.SortBy, parameters.SortDirection);

        return await PagedList<PerformanceReview>.ToPagedListAsync(query, parameters.PageNumber, parameters.PageSize);
    }

    public async Task<PerformanceReview?> GetPerformanceReviewByIdAsync(Guid id, bool trackChanges)
    {
        return await FindByCondition(pr => pr.Id == id, trackChanges)
            .Include(pr => pr.Employee)
            .Include(pr => pr.Reviewer)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<PerformanceReview>> GetPerformanceReviewsByStatusAsync(ReviewStatus status, bool trackChanges)
    {
        return await FindByCondition(pr => pr.Status == status, trackChanges)
            .Include(pr => pr.Employee)
            .Include(pr => pr.Reviewer)
            .ToListAsync();
    }

    public async Task<IEnumerable<PerformanceReview>> GetOverduePerformanceReviewsAsync(bool trackChanges)
    {
        var today = DateTime.UtcNow.Date;
        return await FindByCondition(pr => pr.DueDate < today && 
                                          (pr.Status == ReviewStatus.Draft || pr.Status == ReviewStatus.InProgress), trackChanges)
            .Include(pr => pr.Employee)
            .Include(pr => pr.Reviewer)
            .ToListAsync();
    }

    public async Task<IEnumerable<PerformanceReview>> GetPerformanceReviewsByDateRangeAsync(DateTime startDate, DateTime endDate, bool trackChanges)
    {
        return await FindByCondition(pr => pr.CreatedAt >= startDate && pr.CreatedAt <= endDate, trackChanges)
            .Include(pr => pr.Employee)
            .Include(pr => pr.Reviewer)
            .ToListAsync();
    }

    public void CreatePerformanceReview(PerformanceReview performanceReview) => Create(performanceReview);

    public void UpdatePerformanceReview(PerformanceReview performanceReview) => Update(performanceReview);

    public void DeletePerformanceReview(PerformanceReview performanceReview) => Delete(performanceReview);

    private IQueryable<PerformanceReview> ApplyFilters(IQueryable<PerformanceReview> query, PerformanceReviewParameters parameters)
    {
        if (!string.IsNullOrEmpty(parameters.SearchTerm))
        {
            var searchTerm = parameters.SearchTerm.ToLower();
            query = query.Where(pr => pr.Title.ToLower().Contains(searchTerm) ||
                                     pr.Employee.UserName!.ToLower().Contains(searchTerm) ||
                                     (pr.Description != null && pr.Description.ToLower().Contains(searchTerm)));
        }

        if (parameters.Status.HasValue)
        {
            query = query.Where(pr => pr.Status == parameters.Status.Value);
        }

        if (parameters.Type.HasValue)
        {
            query = query.Where(pr => pr.ReviewType == parameters.Type.Value);
        }

        if (parameters.Priority.HasValue)
        {
            query = query.Where(pr => pr.Priority == parameters.Priority.Value);
        }

        if (parameters.StartDate.HasValue)
        {
            query = query.Where(pr => pr.CreatedAt >= parameters.StartDate.Value);
        }

        if (parameters.EndDate.HasValue)
        {
            query = query.Where(pr => pr.CreatedAt <= parameters.EndDate.Value);
        }

        if (!string.IsNullOrEmpty(parameters.EmployeeId))
        {
            query = query.Where(pr => pr.EmployeeId == parameters.EmployeeId);
        }

        if (!string.IsNullOrEmpty(parameters.ReviewerId))
        {
            query = query.Where(pr => pr.ReviewerId == parameters.ReviewerId);
        }

        return query;
    }

    private IQueryable<PerformanceReview> ApplySorting(IQueryable<PerformanceReview> query, string? sortBy, string? sortDirection)
    {
        var isDescending = sortDirection?.ToLower() == "desc";

        return sortBy?.ToLower() switch
        {
            "title" => isDescending ? query.OrderByDescending(pr => pr.Title) : query.OrderBy(pr => pr.Title),
            "duedate" => isDescending ? query.OrderByDescending(pr => pr.DueDate) : query.OrderBy(pr => pr.DueDate),
            "createddate" => isDescending ? query.OrderByDescending(pr => pr.CreatedAt) : query.OrderBy(pr => pr.CreatedAt),
            "status" => isDescending ? query.OrderByDescending(pr => pr.Status) : query.OrderBy(pr => pr.Status),
            "priority" => isDescending ? query.OrderByDescending(pr => pr.Priority) : query.OrderBy(pr => pr.Priority),
            "progress" => isDescending ? query.OrderByDescending(pr => pr.Progress) : query.OrderBy(pr => pr.Progress),
            "rating" => isDescending ? query.OrderByDescending(pr => pr.Rating) : query.OrderBy(pr => pr.Rating),
            "employeename" => isDescending ? query.OrderByDescending(pr => pr.Employee.UserName) : query.OrderBy(pr => pr.Employee.UserName),
            _ => query.OrderByDescending(pr => pr.CreatedAt)
        };
    }
}
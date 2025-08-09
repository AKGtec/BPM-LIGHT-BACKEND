using AutoMapper;
using ProjectTemplate.Contracts;
using ProjectTemplate.Contracts.Repository;
using ProjectTemplate.Models.Entities;
using ProjectTemplate.Service.Contracts;
using ProjectTemplate.Shared.DataTransferObjects;
using ProjectTemplate.Shared.RequestFeatures;

namespace ProjectTemplate.Service;

public sealed class PerformanceReviewService : IPerformanceReviewService
{
    private readonly IRepositoryManager _repository;
    private readonly ILoggerManager _logger;
    private readonly IMapper _mapper;

    public PerformanceReviewService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
    }

    // Performance Reviews CRUD
    public async Task<PaginatedResponse<PerformanceReviewDto>> GetPerformanceReviewsAsync(PerformanceReviewParameters parameters, bool trackChanges)
    {
        _logger.LogInfo("Getting performance reviews with pagination");
        
        var pagedReviews = await _repository.PerformanceReview.GetAllPerformanceReviewsAsync(parameters, trackChanges);
        var performanceReviewDtos = _mapper.Map<IEnumerable<PerformanceReviewDto>>(pagedReviews);
        
        return new PaginatedResponse<PerformanceReviewDto>(
            performanceReviewDtos,
            pagedReviews.TotalCount,
            pagedReviews.CurrentPage,
            pagedReviews.PageSize
        );
    }

    public async Task<PaginatedResponse<PerformanceReviewDto>> GetMyPerformanceReviewsAsync(string employeeId, PerformanceReviewParameters parameters, bool trackChanges)
    {
        _logger.LogInfo($"Getting performance reviews for employee: {employeeId}");
        
        var pagedReviews = await _repository.PerformanceReview.GetPerformanceReviewsByEmployeeIdAsync(employeeId, parameters, trackChanges);
        var performanceReviewDtos = _mapper.Map<IEnumerable<PerformanceReviewDto>>(pagedReviews);
        
        return new PaginatedResponse<PerformanceReviewDto>(
            performanceReviewDtos,
            pagedReviews.TotalCount,
            pagedReviews.CurrentPage,
            pagedReviews.PageSize
        );
    }

    public async Task<PaginatedResponse<PerformanceReviewDto>> GetPendingPerformanceReviewsAsync(PerformanceReviewParameters parameters, bool trackChanges)
    {
        _logger.LogInfo("Getting pending performance reviews");
        
        var mockData = GenerateMockPerformanceReviews().Where(x => x.Status == ReviewStatus.InProgress || x.Status == ReviewStatus.Draft);
        var filteredData = ApplyFilters(mockData, parameters);
        var pagedData = ApplyPagination(filteredData, parameters);
        
        var performanceReviewDtos = _mapper.Map<IEnumerable<PerformanceReviewDto>>(pagedData);
        
        return new PaginatedResponse<PerformanceReviewDto>(
            performanceReviewDtos,
            filteredData.Count(),
            parameters.PageNumber,
            parameters.PageSize
        );
    }

    public async Task<PerformanceReviewDto?> GetPerformanceReviewByIdAsync(Guid id, bool trackChanges)
    {
        _logger.LogInfo($"Getting performance review by ID: {id}");
        
        var performanceReview = await _repository.PerformanceReview.GetPerformanceReviewByIdAsync(id, trackChanges);
        return performanceReview != null ? _mapper.Map<PerformanceReviewDto>(performanceReview) : null;
    }

    public async Task<PerformanceReviewDto> CreatePerformanceReviewAsync(CreatePerformanceReviewDto createDto, string currentUserId)
    {
        _logger.LogInfo($"Creating performance review for employee: {createDto.EmployeeId}");
        
        // Create the entity
        var performanceReview = new PerformanceReview
        {
            Title = createDto.Title,
            EmployeeId = createDto.EmployeeId,
            ReviewType = createDto.ReviewType,
            Status = ReviewStatus.Draft,
            DueDate = createDto.DueDate,
            Progress = 0,
            Priority = createDto.Priority,
            Description = createDto.Description,
            Goals = createDto.Goals != null ? System.Text.Json.JsonSerializer.Serialize(createDto.Goals) : null,
            ReviewerId = currentUserId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Save to database
        _repository.PerformanceReview.CreatePerformanceReview(performanceReview);
        await _repository.SaveAsync();

        _logger.LogInfo($"Performance review created with ID: {performanceReview.Id}");
        
        // Return the DTO
        return _mapper.Map<PerformanceReviewDto>(performanceReview);
    }

    public async Task<PerformanceReviewDto> UpdatePerformanceReviewAsync(Guid id, UpdatePerformanceReviewDto updateDto, bool trackChanges)
    {
        _logger.LogInfo($"Updating performance review: {id}");
        
        var existingReview = await _repository.PerformanceReview.GetPerformanceReviewByIdAsync(id, trackChanges);
        if (existingReview == null)
            throw new InvalidOperationException($"Performance review with ID {id} not found.");

        // Update properties if provided
        if (!string.IsNullOrEmpty(updateDto.Title))
            existingReview.Title = updateDto.Title;
        
        if (updateDto.ReviewType.HasValue)
            existingReview.ReviewType = updateDto.ReviewType.Value;
        
        if (updateDto.DueDate.HasValue)
            existingReview.DueDate = updateDto.DueDate.Value;
        
        if (updateDto.Priority.HasValue)
            existingReview.Priority = updateDto.Priority.Value;
        
        if (updateDto.Description != null)
            existingReview.Description = updateDto.Description;
        
        if (updateDto.Goals != null)
            existingReview.Goals = System.Text.Json.JsonSerializer.Serialize(updateDto.Goals);
        
        if (updateDto.Progress.HasValue)
            existingReview.Progress = updateDto.Progress.Value;
        
        if (updateDto.Feedback != null)
            existingReview.Feedback = updateDto.Feedback;
        
        if (updateDto.Rating.HasValue)
            existingReview.Rating = updateDto.Rating.Value;

        existingReview.UpdatedAt = DateTime.UtcNow;

        // Save changes
        await _repository.SaveAsync();

        return _mapper.Map<PerformanceReviewDto>(existingReview);
    }

    public async Task DeletePerformanceReviewAsync(Guid id, bool trackChanges)
    {
        _logger.LogInfo($"Deleting performance review: {id}");
        
        var review = await _repository.PerformanceReview.GetPerformanceReviewByIdAsync(id, trackChanges);
        if (review == null)
            throw new InvalidOperationException($"Performance review with ID {id} not found.");
        
        _repository.PerformanceReview.DeletePerformanceReview(review);
        await _repository.SaveAsync();
    }

    // Review Actions
    public async Task StartReviewAsync(Guid id, string currentUserId)
    {
        _logger.LogInfo($"Starting performance review: {id} by user: {currentUserId}");
        
        // In real implementation, update database
        // var review = await _repository.PerformanceReview.GetByIdAsync(id, trackChanges: true);
        // review.Status = ReviewStatus.InProgress;
        // review.UpdatedAt = DateTime.UtcNow;
        // await _repository.SaveAsync();
    }

    public async Task CompleteReviewAsync(Guid id, CompleteReviewDto completeDto, string currentUserId)
    {
        _logger.LogInfo($"Completing performance review: {id} by user: {currentUserId}");
        
        // In real implementation, update database
        // var review = await _repository.PerformanceReview.GetByIdAsync(id, trackChanges: true);
        // review.Status = ReviewStatus.Completed;
        // review.Feedback = completeDto.Feedback;
        // review.Rating = completeDto.Rating;
        // review.CompletedDate = DateTime.UtcNow;
        // review.Progress = 100;
        // review.UpdatedAt = DateTime.UtcNow;
        // await _repository.SaveAsync();
    }

    public async Task CancelReviewAsync(Guid id, CancelReviewDto cancelDto, string currentUserId)
    {
        _logger.LogInfo($"Cancelling performance review: {id} by user: {currentUserId}");
        
        // In real implementation, update database
        // var review = await _repository.PerformanceReview.GetByIdAsync(id, trackChanges: true);
        // review.Status = ReviewStatus.Cancelled;
        // review.UpdatedAt = DateTime.UtcNow;
        // await _repository.SaveAsync();
    }

    // Bulk Operations
    public async Task BulkUpdateStatusAsync(BulkUpdateStatusDto bulkUpdateDto, string currentUserId)
    {
        _logger.LogInfo($"Bulk updating status for {bulkUpdateDto.Ids.Count} performance reviews by user: {currentUserId}");
        
        // In real implementation, update multiple records in database
        foreach (var id in bulkUpdateDto.Ids)
        {
            // Update each review status
        }
    }

    public async Task BulkDeleteAsync(BulkDeleteDto bulkDeleteDto, string currentUserId)
    {
        _logger.LogInfo($"Bulk deleting {bulkDeleteDto.Ids.Count} performance reviews by user: {currentUserId}");
        
        // In real implementation, delete multiple records from database
        foreach (var id in bulkDeleteDto.Ids)
        {
            await DeletePerformanceReviewAsync(id, trackChanges: false);
        }
    }

    // Statistics and Reports
    public async Task<PerformanceReviewStatisticsDto> GetPerformanceReviewStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        _logger.LogInfo($"Getting performance review statistics from {startDate} to {endDate}");
        
        var mockReviews = GenerateMockPerformanceReviews();
        
        if (startDate.HasValue)
            mockReviews = mockReviews.Where(x => x.CreatedDate >= startDate.Value);
        
        if (endDate.HasValue)
            mockReviews = mockReviews.Where(x => x.CreatedDate <= endDate.Value);

        var totalReviews = mockReviews.Count();
        var completedReviews = mockReviews.Count(x => x.Status == ReviewStatus.Completed);
        var overdueReviews = mockReviews.Count(x => x.Status == ReviewStatus.Overdue || (x.DueDate < DateTime.UtcNow && x.Status != ReviewStatus.Completed));

        return new PerformanceReviewStatisticsDto
        {
            TotalReviews = totalReviews,
            PendingReviews = mockReviews.Count(x => x.Status == ReviewStatus.InProgress || x.Status == ReviewStatus.Draft),
            CompletedReviews = completedReviews,
            OverdueReviews = overdueReviews,
            AverageRating = mockReviews.Where(x => x.Rating.HasValue).Average(x => x.Rating.Value),
            CompletionRate = totalReviews > 0 ? (decimal)completedReviews / totalReviews * 100 : 0
        };
    }

    public async Task<byte[]> ExportPerformanceReviewsAsync(string format, DateTime? startDate = null, DateTime? endDate = null)
    {
        _logger.LogInfo($"Exporting performance reviews in {format} format");
        
        // In real implementation, generate Excel or PDF report
        var mockData = "Mock performance review report data";
        return System.Text.Encoding.UTF8.GetBytes(mockData);
    }

    // Utility methods
    public string GetReviewTypeLabel(ReviewType type)
    {
        return type switch
        {
            ReviewType.Annual => "Annual Review",
            ReviewType.Quarterly => "Quarterly Review",
            ReviewType.Probationary => "Probationary Review",
            ReviewType.Project => "Project Review",
            ReviewType.ThreeSixty => "360-Degree Review",
            ReviewType.MidYear => "Mid-Year Review",
            _ => "Unknown"
        };
    }

    public string GetReviewStatusLabel(ReviewStatus status)
    {
        return status switch
        {
            ReviewStatus.Draft => "Draft",
            ReviewStatus.InProgress => "In Progress",
            ReviewStatus.Completed => "Completed",
            ReviewStatus.Overdue => "Overdue",
            ReviewStatus.Cancelled => "Cancelled",
            _ => "Unknown"
        };
    }

    public string GetReviewPriorityLabel(ReviewPriority priority)
    {
        return priority switch
        {
            ReviewPriority.Low => "Low",
            ReviewPriority.Medium => "Medium",
            ReviewPriority.High => "High",
            ReviewPriority.Critical => "Critical",
            _ => "Unknown"
        };
    }

    public IEnumerable<ReviewTypeOptionDto> GetReviewTypeOptions()
    {
        return new List<ReviewTypeOptionDto>
        {
            new() { Value = ReviewType.Annual, Label = GetReviewTypeLabel(ReviewType.Annual) },
            new() { Value = ReviewType.Quarterly, Label = GetReviewTypeLabel(ReviewType.Quarterly) },
            new() { Value = ReviewType.Probationary, Label = GetReviewTypeLabel(ReviewType.Probationary) },
            new() { Value = ReviewType.Project, Label = GetReviewTypeLabel(ReviewType.Project) },
            new() { Value = ReviewType.ThreeSixty, Label = GetReviewTypeLabel(ReviewType.ThreeSixty) },
            new() { Value = ReviewType.MidYear, Label = GetReviewTypeLabel(ReviewType.MidYear) }
        };
    }

    public IEnumerable<ReviewStatusOptionDto> GetReviewStatusOptions()
    {
        return new List<ReviewStatusOptionDto>
        {
            new() { Value = ReviewStatus.Draft, Label = GetReviewStatusLabel(ReviewStatus.Draft) },
            new() { Value = ReviewStatus.InProgress, Label = GetReviewStatusLabel(ReviewStatus.InProgress) },
            new() { Value = ReviewStatus.Completed, Label = GetReviewStatusLabel(ReviewStatus.Completed) },
            new() { Value = ReviewStatus.Overdue, Label = GetReviewStatusLabel(ReviewStatus.Overdue) },
            new() { Value = ReviewStatus.Cancelled, Label = GetReviewStatusLabel(ReviewStatus.Cancelled) }
        };
    }

    public IEnumerable<ReviewPriorityOptionDto> GetReviewPriorityOptions()
    {
        return new List<ReviewPriorityOptionDto>
        {
            new() { Value = ReviewPriority.Low, Label = GetReviewPriorityLabel(ReviewPriority.Low) },
            new() { Value = ReviewPriority.Medium, Label = GetReviewPriorityLabel(ReviewPriority.Medium) },
            new() { Value = ReviewPriority.High, Label = GetReviewPriorityLabel(ReviewPriority.High) },
            new() { Value = ReviewPriority.Critical, Label = GetReviewPriorityLabel(ReviewPriority.Critical) }
        };
    }

    // Private helper methods for mock data
    private IEnumerable<PerformanceReviewDto> GenerateMockPerformanceReviews()
    {
        return new List<PerformanceReviewDto>
        {
            new PerformanceReviewDto
            {
                Id = Guid.NewGuid(),
                Title = "Annual Performance Review 2024",
                EmployeeId = "emp001",
                EmployeeName = "John Doe",
                EmployeePosition = "Software Developer",
                ReviewType = ReviewType.Annual,
                Status = ReviewStatus.InProgress,
                DueDate = DateTime.Today.AddDays(30),
                CreatedDate = DateTime.Today.AddDays(-60),
                Progress = 75,
                Priority = ReviewPriority.High,
                Description = "Annual performance evaluation for 2024",
                Goals = new List<string> { "Improve code quality", "Learn new technologies", "Mentor junior developers" },
                ReviewerId = "mgr001",
                ReviewerName = "Jane Manager",
                CreatedAt = DateTime.Today.AddDays(-60),
                UpdatedAt = DateTime.Today.AddDays(-1)
            },
            new PerformanceReviewDto
            {
                Id = Guid.NewGuid(),
                Title = "Q4 Quarterly Review",
                EmployeeId = "emp002",
                EmployeeName = "Alice Smith",
                EmployeePosition = "Senior Developer",
                ReviewType = ReviewType.Quarterly,
                Status = ReviewStatus.Completed,
                DueDate = DateTime.Today.AddDays(-10),
                CreatedDate = DateTime.Today.AddDays(-45),
                Progress = 100,
                Priority = ReviewPriority.Medium,
                Description = "Quarterly performance review for Q4",
                Goals = new List<string> { "Complete project deliverables", "Improve team collaboration" },
                Feedback = "Excellent performance this quarter. Met all objectives.",
                Rating = 4.5m,
                ReviewerId = "mgr001",
                ReviewerName = "Jane Manager",
                CompletedDate = DateTime.Today.AddDays(-5),
                CreatedAt = DateTime.Today.AddDays(-45),
                UpdatedAt = DateTime.Today.AddDays(-5)
            },
            new PerformanceReviewDto
            {
                Id = Guid.NewGuid(),
                Title = "Probationary Review",
                EmployeeId = "emp003",
                EmployeeName = "Bob Johnson",
                EmployeePosition = "Junior Developer",
                ReviewType = ReviewType.Probationary,
                Status = ReviewStatus.Draft,
                DueDate = DateTime.Today.AddDays(15),
                CreatedDate = DateTime.Today.AddDays(-30),
                Progress = 25,
                Priority = ReviewPriority.Critical,
                Description = "3-month probationary review",
                Goals = new List<string> { "Complete onboarding", "Learn company processes", "Deliver first project" },
                ReviewerId = "mgr002",
                ReviewerName = "Mike Supervisor",
                CreatedAt = DateTime.Today.AddDays(-30),
                UpdatedAt = DateTime.Today.AddDays(-2)
            }
        };
    }

    private IEnumerable<PerformanceReviewDto> ApplyFilters(IEnumerable<PerformanceReviewDto> data, PerformanceReviewParameters parameters)
    {
        if (!string.IsNullOrEmpty(parameters.SearchTerm))
        {
            data = data.Where(x => x.Title.Contains(parameters.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                                  x.EmployeeName.Contains(parameters.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                                  (x.Description != null && x.Description.Contains(parameters.SearchTerm, StringComparison.OrdinalIgnoreCase)));
        }

        if (parameters.Status.HasValue)
        {
            data = data.Where(x => x.Status == parameters.Status.Value);
        }

        if (parameters.Type.HasValue)
        {
            data = data.Where(x => x.ReviewType == parameters.Type.Value);
        }

        if (parameters.Priority.HasValue)
        {
            data = data.Where(x => x.Priority == parameters.Priority.Value);
        }

        if (parameters.StartDate.HasValue)
        {
            data = data.Where(x => x.CreatedDate >= parameters.StartDate.Value);
        }

        if (parameters.EndDate.HasValue)
        {
            data = data.Where(x => x.CreatedDate <= parameters.EndDate.Value);
        }

        if (!string.IsNullOrEmpty(parameters.EmployeeId))
        {
            data = data.Where(x => x.EmployeeId == parameters.EmployeeId);
        }

        if (!string.IsNullOrEmpty(parameters.ReviewerId))
        {
            data = data.Where(x => x.ReviewerId == parameters.ReviewerId);
        }

        return data;
    }

    private IEnumerable<PerformanceReviewDto> ApplyPagination(IEnumerable<PerformanceReviewDto> data, PerformanceReviewParameters parameters)
    {
        return data.Skip((parameters.PageNumber - 1) * parameters.PageSize)
                   .Take(parameters.PageSize);
    }
}
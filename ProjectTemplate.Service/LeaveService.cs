using AutoMapper;
using ProjectTemplate.Contracts;
using ProjectTemplate.Contracts.Repository;
using ProjectTemplate.Models.Entities;
using ProjectTemplate.Service.Contracts;
using ProjectTemplate.Shared.DataTransferObjects;
using ProjectTemplate.Shared.RequestFeatures;

namespace ProjectTemplate.Service;

public sealed class LeaveService : ILeaveService
{
    private readonly IRepositoryManager _repository;
    private readonly ILoggerManager _logger;
    private readonly IMapper _mapper;

    public LeaveService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
    }

    // Leave Requests
    public async Task<PaginatedResponse<LeaveRequestDto>> GetLeaveRequestsAsync(LeaveRequestParameters parameters, bool trackChanges)
    {
        _logger.LogInfo("Getting leave requests with pagination");
        
        var pagedLeaves = await _repository.Leave.GetAllLeavesAsync(parameters, trackChanges);
        var leaveRequestDtos = _mapper.Map<IEnumerable<LeaveRequestDto>>(pagedLeaves);
        
        return new PaginatedResponse<LeaveRequestDto>(
            leaveRequestDtos,
            pagedLeaves.TotalCount,
            pagedLeaves.CurrentPage,
            pagedLeaves.PageSize
        );
    }

    public async Task<PaginatedResponse<LeaveRequestDto>> GetPendingLeaveRequestsAsync(LeaveRequestParameters parameters, bool trackChanges)
    {
        _logger.LogInfo("Getting pending leave requests");
        
        var pagedLeaves = await _repository.Leave.GetPendingLeavesAsync(parameters, trackChanges);
        var leaveRequestDtos = _mapper.Map<IEnumerable<LeaveRequestDto>>(pagedLeaves);
        
        return new PaginatedResponse<LeaveRequestDto>(
            leaveRequestDtos,
            pagedLeaves.TotalCount,
            pagedLeaves.CurrentPage,
            pagedLeaves.PageSize
        );
    }

    public async Task<PaginatedResponse<LeaveRequestDto>> GetMyLeaveRequestsAsync(string employeeId, LeaveRequestParameters parameters, bool trackChanges)
    {
        _logger.LogInfo($"Getting leave requests for employee: {employeeId}");
        
        var pagedLeaves = await _repository.Leave.GetLeavesByEmployeeIdAsync(employeeId, parameters, trackChanges);
        var leaveRequestDtos = _mapper.Map<IEnumerable<LeaveRequestDto>>(pagedLeaves);
        
        return new PaginatedResponse<LeaveRequestDto>(
            leaveRequestDtos,
            pagedLeaves.TotalCount,
            pagedLeaves.CurrentPage,
            pagedLeaves.PageSize
        );
    }

    public async Task<LeaveRequestDto?> GetLeaveRequestByIdAsync(Guid id, bool trackChanges)
    {
        _logger.LogInfo($"Getting leave request by ID: {id}");
        
        var leave = await _repository.Leave.GetLeaveByIdAsync(id, trackChanges);
        return leave != null ? _mapper.Map<LeaveRequestDto>(leave) : null;
    }

    public async Task<LeaveRequestDto> CreateLeaveRequestAsync(CreateLeaveRequestDto createDto, string currentUserId)
    {
        _logger.LogInfo($"Creating leave request for employee: {createDto.EmployeeId}");
        
        var leave = _mapper.Map<Leave>(createDto);
        leave.EmployeeId = createDto.EmployeeId;
        leave.Days = CalculateLeaveDays(createDto.StartDate, createDto.EndDate);
        
        _repository.Leave.CreateLeave(leave);
        await _repository.SaveAsync();
        
        // Reload with navigation properties
        var createdLeave = await _repository.Leave.GetLeaveByIdAsync(leave.Id, trackChanges: false);
        var leaveRequestDto = _mapper.Map<LeaveRequestDto>(createdLeave);
        
        _logger.LogInfo($"Leave request created with ID: {leave.Id}");
        
        return leaveRequestDto;
    }

    public async Task ApproveLeaveRequestAsync(Guid id, ApproveRejectLeaveDto data, string approverId)
    {
        _logger.LogInfo($"Approving leave request: {id} by user: {approverId}");
        
        var leave = await _repository.Leave.GetLeaveByIdAsync(id, trackChanges: true);
        if (leave == null)
            throw new ArgumentException($"Leave request with ID {id} not found");
            
        leave.Status = LeaveStatus.Approved;
        leave.ApprovedBy = approverId;
        leave.ApprovedDate = DateTime.UtcNow;
        leave.Comments = data.Comments;
        leave.UpdatedAt = DateTime.UtcNow;
        
        _repository.Leave.UpdateLeave(leave);
        await _repository.SaveAsync();
    }

    public async Task RejectLeaveRequestAsync(Guid id, ApproveRejectLeaveDto data, string rejectorId)
    {
        _logger.LogInfo($"Rejecting leave request: {id} by user: {rejectorId}");
        
        var leave = await _repository.Leave.GetLeaveByIdAsync(id, trackChanges: true);
        if (leave == null)
            throw new ArgumentException($"Leave request with ID {id} not found");
            
        leave.Status = LeaveStatus.Rejected;
        leave.RejectedBy = rejectorId;
        leave.RejectedDate = DateTime.UtcNow;
        leave.RejectionReason = data.Comments;
        leave.UpdatedAt = DateTime.UtcNow;
        
        _repository.Leave.UpdateLeave(leave);
        await _repository.SaveAsync();
    }

    public async Task CancelLeaveRequestAsync(Guid id, string currentUserId)
    {
        _logger.LogInfo($"Cancelling leave request: {id} by user: {currentUserId}");
        
        var leave = await _repository.Leave.GetLeaveByIdAsync(id, trackChanges: true);
        if (leave == null)
            throw new ArgumentException($"Leave request with ID {id} not found");
            
        leave.Status = LeaveStatus.Cancelled;
        leave.UpdatedAt = DateTime.UtcNow;
        
        _repository.Leave.UpdateLeave(leave);
        await _repository.SaveAsync();
    }

    public async Task BulkApproveLeaveRequestsAsync(BulkApproveRejectLeaveDto data, string approverId)
    {
        _logger.LogInfo($"Bulk approving {data.Ids.Count} leave requests by user: {approverId}");
        
        // In real implementation, update multiple records in database
        foreach (var id in data.Ids)
        {
            await ApproveLeaveRequestAsync(id, new ApproveRejectLeaveDto { Comments = data.Comments }, approverId);
        }
    }

    public async Task BulkRejectLeaveRequestsAsync(BulkApproveRejectLeaveDto data, string rejectorId)
    {
        _logger.LogInfo($"Bulk rejecting {data.Ids.Count} leave requests by user: {rejectorId}");
        
        // In real implementation, update multiple records in database
        foreach (var id in data.Ids)
        {
            await RejectLeaveRequestAsync(id, new ApproveRejectLeaveDto { Comments = data.Comments }, rejectorId);
        }
    }

    // Leave Balances
    public async Task<IEnumerable<LeaveBalanceDto>> GetLeaveBalancesAsync(string? employeeId = null)
    {
        _logger.LogInfo($"Getting leave balances for employee: {employeeId ?? "all employees"}");
        
        IEnumerable<LeaveBalance> balances;
        if (!string.IsNullOrEmpty(employeeId))
        {
            balances = await _repository.LeaveBalance.GetLeaveBalancesByEmployeeIdAsync(employeeId, trackChanges: false);
        }
        else
        {
            balances = await _repository.LeaveBalance.GetAllLeaveBalancesAsync(trackChanges: false);
        }
        
        return _mapper.Map<IEnumerable<LeaveBalanceDto>>(balances);
    }

    public async Task<IEnumerable<LeaveBalanceDto>> GetMyLeaveBalancesAsync(string employeeId)
    {
        _logger.LogInfo($"Getting leave balances for employee: {employeeId}");
        
        var balances = await _repository.LeaveBalance.GetLeaveBalancesByEmployeeIdAsync(employeeId, trackChanges: false);
        return _mapper.Map<IEnumerable<LeaveBalanceDto>>(balances);
    }

    public async Task UpdateLeaveBalanceAsync(UpdateLeaveBalanceDto updateDto)
    {
        _logger.LogInfo($"Updating leave balance for employee: {updateDto.EmployeeId}");
        
        var balance = await _repository.LeaveBalance.GetLeaveBalanceAsync(
            updateDto.EmployeeId, 
            updateDto.LeaveType, 
            updateDto.Year, 
            trackChanges: true);
            
        if (balance == null)
        {
            // Create new balance if doesn't exist
            balance = new LeaveBalance
            {
                EmployeeId = updateDto.EmployeeId,
                LeaveType = updateDto.LeaveType,
                Year = updateDto.Year,
                TotalDays = updateDto.TotalDays,
                UsedDays = 0,
                RemainingDays = updateDto.TotalDays
            };
            _repository.LeaveBalance.CreateLeaveBalance(balance);
        }
        else
        {
            _mapper.Map(updateDto, balance);
            balance.RemainingDays = balance.TotalDays - balance.UsedDays;
            _repository.LeaveBalance.UpdateLeaveBalance(balance);
        }
        
        await _repository.SaveAsync();
    }

    // Leave Types Configuration
    public async Task<IEnumerable<LeaveTypeConfigDto>> GetLeaveTypesAsync()
    {
        _logger.LogInfo("Getting leave types configuration");
        
        return GenerateMockLeaveTypes();
    }

    // Statistics and Reports
    public async Task<LeaveStatisticsDto> GetLeaveStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        _logger.LogInfo($"Getting leave statistics from {startDate} to {endDate}");
        
        var mockRequests = GenerateMockLeaveRequests();
        
        if (startDate.HasValue)
            mockRequests = mockRequests.Where(x => x.StartDate >= startDate.Value);
        
        if (endDate.HasValue)
            mockRequests = mockRequests.Where(x => x.EndDate <= endDate.Value);

        return new LeaveStatisticsDto
        {
            TotalRequests = mockRequests.Count(),
            PendingRequests = mockRequests.Count(x => x.Status == LeaveStatus.Pending),
            ApprovedRequests = mockRequests.Count(x => x.Status == LeaveStatus.Approved),
            RejectedRequests = mockRequests.Count(x => x.Status == LeaveStatus.Rejected),
            TotalDaysRequested = mockRequests.Sum(x => x.Days),
            TotalDaysApproved = mockRequests.Where(x => x.Status == LeaveStatus.Approved).Sum(x => x.Days),
            MostRequestedLeaveType = mockRequests.GroupBy(x => x.LeaveType)
                .OrderByDescending(g => g.Count())
                .FirstOrDefault()?.Key.ToString() ?? "None"
        };
    }

    public async Task<byte[]> ExportLeaveReportAsync(string format, DateTime? startDate = null, DateTime? endDate = null)
    {
        _logger.LogInfo($"Exporting leave report in {format} format");
        
        // In real implementation, generate Excel or PDF report
        var mockData = "Mock report data";
        return System.Text.Encoding.UTF8.GetBytes(mockData);
    }

    // Utility methods
    public string GetLeaveTypeLabel(LeaveType type)
    {
        return type switch
        {
            LeaveType.AnnualLeave => "Annual Leave",
            LeaveType.SickLeave => "Sick Leave",
            LeaveType.PersonalLeave => "Personal Leave",
            LeaveType.MaternityLeave => "Maternity Leave",
            LeaveType.PaternityLeave => "Paternity Leave",
            LeaveType.EmergencyLeave => "Emergency Leave",
            LeaveType.StudyLeave => "Study Leave",
            LeaveType.UnpaidLeave => "Unpaid Leave",
            _ => "Unknown"
        };
    }

    public string GetLeaveStatusLabel(LeaveStatus status)
    {
        return status switch
        {
            LeaveStatus.Pending => "Pending",
            LeaveStatus.Approved => "Approved",
            LeaveStatus.Rejected => "Rejected",
            LeaveStatus.Cancelled => "Cancelled",
            _ => "Unknown"
        };
    }

    public int CalculateLeaveDays(DateTime startDate, DateTime endDate)
    {
        var timeDiff = endDate - startDate;
        return (int)Math.Ceiling(timeDiff.TotalDays) + 1;
    }

    // Private helper methods for mock data
    private IEnumerable<LeaveRequestDto> GenerateMockLeaveRequests()
    {
        return new List<LeaveRequestDto>
        {
            new LeaveRequestDto
            {
                Id = Guid.NewGuid(),
                EmployeeId = "emp001",
                EmployeeName = "John Doe",
                EmployeeEmail = "john.doe@company.com",
                LeaveType = LeaveType.AnnualLeave,
                StartDate = DateTime.Today.AddDays(10),
                EndDate = DateTime.Today.AddDays(15),
                Days = 6,
                Reason = "Family vacation",
                Status = LeaveStatus.Pending,
                SubmittedDate = DateTime.Today,
                CreatedAt = DateTime.Today,
                UpdatedAt = DateTime.Today
            },
            new LeaveRequestDto
            {
                Id = Guid.NewGuid(),
                EmployeeId = "emp002",
                EmployeeName = "Jane Smith",
                EmployeeEmail = "jane.smith@company.com",
                LeaveType = LeaveType.SickLeave,
                StartDate = DateTime.Today.AddDays(-5),
                EndDate = DateTime.Today.AddDays(-3),
                Days = 3,
                Reason = "Medical appointment",
                Status = LeaveStatus.Approved,
                SubmittedDate = DateTime.Today.AddDays(-7),
                ApprovedBy = "manager001",
                ApprovedDate = DateTime.Today.AddDays(-6),
                CreatedAt = DateTime.Today.AddDays(-7),
                UpdatedAt = DateTime.Today.AddDays(-6)
            }
        };
    }

    private IEnumerable<LeaveBalanceDto> GenerateMockLeaveBalances(string? employeeId)
    {
        var balances = new List<LeaveBalanceDto>
        {
            new LeaveBalanceDto
            {
                EmployeeId = "emp001",
                EmployeeName = "John Doe",
                LeaveType = LeaveType.AnnualLeave,
                TotalDays = 25,
                UsedDays = 5,
                RemainingDays = 20,
                Year = DateTime.Now.Year
            },
            new LeaveBalanceDto
            {
                EmployeeId = "emp001",
                EmployeeName = "John Doe",
                LeaveType = LeaveType.SickLeave,
                TotalDays = 10,
                UsedDays = 2,
                RemainingDays = 8,
                Year = DateTime.Now.Year
            }
        };

        return employeeId != null ? balances.Where(x => x.EmployeeId == employeeId) : balances;
    }

    private IEnumerable<LeaveTypeConfigDto> GenerateMockLeaveTypes()
    {
        return new List<LeaveTypeConfigDto>
        {
            new LeaveTypeConfigDto
            {
                Id = Guid.NewGuid(),
                Name = "Annual Leave",
                MaxDaysPerYear = 25,
                RequiresApproval = true,
                AllowCarryOver = true,
                IsActive = true
            },
            new LeaveTypeConfigDto
            {
                Id = Guid.NewGuid(),
                Name = "Sick Leave",
                MaxDaysPerYear = 10,
                RequiresApproval = false,
                AllowCarryOver = false,
                IsActive = true
            }
        };
    }

    private IEnumerable<LeaveRequestDto> ApplyFilters(IEnumerable<LeaveRequestDto> data, LeaveRequestParameters parameters)
    {
        if (!string.IsNullOrEmpty(parameters.SearchTerm))
        {
            data = data.Where(x => x.EmployeeName.Contains(parameters.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                                  x.Reason.Contains(parameters.SearchTerm, StringComparison.OrdinalIgnoreCase));
        }

        if (parameters.Status.HasValue)
        {
            data = data.Where(x => x.Status == parameters.Status.Value);
        }

        if (parameters.Type.HasValue)
        {
            data = data.Where(x => x.LeaveType == parameters.Type.Value);
        }

        if (parameters.StartDate.HasValue)
        {
            data = data.Where(x => x.StartDate >= parameters.StartDate.Value);
        }

        if (parameters.EndDate.HasValue)
        {
            data = data.Where(x => x.EndDate <= parameters.EndDate.Value);
        }

        if (!string.IsNullOrEmpty(parameters.EmployeeId))
        {
            data = data.Where(x => x.EmployeeId == parameters.EmployeeId);
        }

        return data;
    }

    private IEnumerable<LeaveRequestDto> ApplyPagination(IEnumerable<LeaveRequestDto> data, LeaveRequestParameters parameters)
    {
        return data.Skip((parameters.PageNumber - 1) * parameters.PageSize)
                   .Take(parameters.PageSize);
    }
}
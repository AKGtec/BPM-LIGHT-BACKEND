using Microsoft.AspNetCore.Http;
using ProjectTemplate.Models.Entities;

namespace ProjectTemplate.Presentation.Models;

public class CreateLeaveRequestWithFilesDto
{
    public string EmployeeId { get; set; } = string.Empty;
    public LeaveType LeaveType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Reason { get; set; } = string.Empty;
    public List<IFormFile>? Attachments { get; set; }
}
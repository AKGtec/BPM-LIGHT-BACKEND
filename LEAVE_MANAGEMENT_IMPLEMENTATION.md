# Leave Management System Implementation

This document describes the complete Leave Management system implementation based on the provided TypeScript service.

## Overview

The Leave Management system provides comprehensive functionality for managing employee leave requests, balances, approvals, and reporting. The implementation follows the existing project architecture patterns and includes:

- Leave request management (CRUD operations)
- Leave balance tracking
- Approval/rejection workflows
- Bulk operations
- Statistics and reporting
- File attachment support

## Files Created/Modified

### 1. Data Transfer Objects (DTOs)
**File:** `ProjectTemplate.Shared/DataTransferObjects/LeaveDtos.cs`

Contains all the DTOs for Leave management:
- `LeaveRequestDto` - Main leave request data structure
- `CreateLeaveRequestDto` - For creating new leave requests
- `ApproveRejectLeaveDto` - For approval/rejection actions
- `BulkApproveRejectLeaveDto` - For bulk operations
- `LeaveBalanceDto` - Leave balance information
- `LeaveTypeConfigDto` - Leave type configuration
- `LeaveStatisticsDto` - Statistics data
- `UpdateLeaveBalanceDto` - For balance adjustments
- `LeaveRequestParameters` - Query parameters for filtering

### 2. Enums
**File:** `ProjectTemplate.Models/Entities/Enums.cs` (Modified)

Added new enums:
- `LeaveType` - Types of leave (Annual, Sick, Personal, etc.)
- `LeaveStatus` - Status of leave requests (Pending, Approved, Rejected, Cancelled)

### 3. Service Interface
**File:** `ProjectTemplate.Service.Contracts/ILeaveService.cs`

Defines the contract for Leave service operations:
- Leave request management methods
- Leave balance operations
- Statistics and reporting
- Utility methods for labels and calculations

### 4. Service Implementation
**File:** `ProjectTemplate.Service/LeaveService.cs`

Complete implementation of the Leave service with:
- Mock data generation (since no database changes were requested)
- All CRUD operations for leave requests
- Balance management
- Statistics calculation
- Filtering and pagination support
- Utility methods

### 5. Controller
**File:** `ProjectTemplate.Presentation/Controllers/LeaveController.cs`

RESTful API controller with endpoints:
- `GET /api/Leave` - Get all leave requests with filtering
- `GET /api/Leave/pending` - Get pending requests
- `GET /api/Leave/my-requests` - Get current user's requests
- `GET /api/Leave/{id}` - Get specific request
- `POST /api/Leave` - Create new request (JSON)
- `POST /api/Leave/with-files` - Create request with file uploads
- `POST /api/Leave/{id}/approve` - Approve request
- `POST /api/Leave/{id}/reject` - Reject request
- `POST /api/Leave/{id}/cancel` - Cancel request
- `POST /api/Leave/bulk-approve` - Bulk approve
- `POST /api/Leave/bulk-reject` - Bulk reject
- `GET /api/Leave/balances` - Get leave balances
- `GET /api/Leave/my-balances` - Get user's balances
- `POST /api/Leave/balances/adjust` - Adjust balance
- `GET /api/Leave/types` - Get leave types
- `GET /api/Leave/statistics` - Get statistics
- `GET /api/Leave/export` - Export reports
- Utility endpoints for labels and calculations

### 6. Form DTOs for File Uploads
**File:** `ProjectTemplate.Presentation/Models/LeaveFormDtos.cs`

Contains DTOs that support file uploads:
- `CreateLeaveRequestWithFilesDto` - For multipart form data with files

### 7. Service Manager Updates
**File:** `ProjectTemplate.Service/ServiceManager.cs` (Modified)
**File:** `ProjectTemplate.Service.Contracts/IServiceManager.cs` (Modified)

Added LeaveService to the service manager for dependency injection.

### 8. AutoMapper Profile
**File:** `ProjectTemplate.Service/MappingProfile.cs` (Modified)

Added placeholder for future Leave entity mappings.

### 9. Test Endpoints
**File:** `test-leave-endpoints.http`

Comprehensive HTTP test file with examples for all endpoints including:
- Different parameter combinations
- File upload examples
- Bulk operations
- All CRUD operations

## API Endpoints Summary

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/Leave` | Get all leave requests with pagination/filtering |
| GET | `/api/Leave/pending` | Get pending leave requests |
| GET | `/api/Leave/my-requests` | Get current user's leave requests |
| GET | `/api/Leave/{id}` | Get leave request by ID |
| POST | `/api/Leave` | Create new leave request (JSON) |
| POST | `/api/Leave/with-files` | Create leave request with files |
| POST | `/api/Leave/{id}/approve` | Approve leave request |
| POST | `/api/Leave/{id}/reject` | Reject leave request |
| POST | `/api/Leave/{id}/cancel` | Cancel leave request |
| POST | `/api/Leave/bulk-approve` | Bulk approve requests |
| POST | `/api/Leave/bulk-reject` | Bulk reject requests |
| GET | `/api/Leave/balances` | Get leave balances |
| GET | `/api/Leave/my-balances` | Get user's leave balances |
| POST | `/api/Leave/balances/adjust` | Adjust leave balance |
| GET | `/api/Leave/types` | Get leave type configurations |
| GET | `/api/Leave/statistics` | Get leave statistics |
| GET | `/api/Leave/export` | Export leave report |
| GET | `/api/Leave/types/{type}/label` | Get leave type label |
| GET | `/api/Leave/status/{status}/label` | Get leave status label |
| GET | `/api/Leave/calculate-days` | Calculate leave days |

## Features Implemented

### 1. Leave Request Management
- Create, read, update, delete leave requests
- Support for different leave types
- File attachment support
- Status tracking (Pending, Approved, Rejected, Cancelled)

### 2. Approval Workflow
- Individual approval/rejection
- Bulk approval/rejection
- Comments support
- Approval tracking (who approved/rejected and when)

### 3. Leave Balance Management
- Track leave balances by type and employee
- Balance adjustments
- Remaining days calculation

### 4. Filtering and Pagination
- Search by employee name or reason
- Filter by status, type, date range
- Pagination support
- Sorting capabilities

### 5. Statistics and Reporting
- Request counts by status
- Days requested/approved totals
- Most requested leave type
- Export functionality (Excel/PDF placeholder)

### 6. Utility Functions
- Leave type and status label conversion
- Leave days calculation
- Date range validation

## Mock Data

Since no database changes were requested, the service uses mock data to demonstrate functionality:
- Sample leave requests with different statuses
- Mock leave balances for employees
- Sample leave type configurations
- Generated statistics

## Authentication & Authorization

The controller uses JWT authentication and extracts user information from claims:
- `User.FindFirst(ClaimTypes.NameIdentifier)?.Value` for user ID
- All endpoints require authentication
- User-specific operations filter by current user

## Error Handling

Comprehensive error handling with:
- Input validation
- Null checks
- Exception catching with proper HTTP status codes
- Meaningful error messages

## Testing

Use the provided `test-leave-endpoints.http` file to test all endpoints. Replace `{{$guid}}` with actual GUIDs and `{{token}}` with a valid JWT token.

## Future Enhancements

When database entities are added:
1. Create Leave entity models
2. Add repository interfaces and implementations
3. Update AutoMapper profiles
4. Replace mock data with actual database operations
5. Add proper file storage for attachments
6. Implement actual report generation

## Notes

- All code follows the existing project patterns and conventions
- No database changes were made as requested
- File uploads are supported but files are not actually stored
- Export functionality returns mock data
- All endpoints are fully functional with mock data
- The implementation is ready for database integration when needed
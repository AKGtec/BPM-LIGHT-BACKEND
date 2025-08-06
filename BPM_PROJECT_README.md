# Business Process Automation Platform

A complete .NET 8 Web API for business process automation built using Clean Architecture principles with Entity Framework Core Code First approach.

## 🏗️ Architecture Overview

This project follows Clean Architecture with the following layers:

### Core Layer
- **ProjectTemplate.Models**: Domain entities, enums, and data context
- **ProjectTemplate.Contracts**: Repository interfaces and contracts
- **ProjectTemplate.Shared**: DTOs, request parameters, and shared models

### Infrastructure Layer
- **ProjectTemplate.Repository**: Data access implementations using EF Core
- **ProjectTemplate.LoggerService**: Logging service implementation

### Application Layer
- **ProjectTemplate.Service.Contracts**: Business logic service interfaces
- **ProjectTemplate.Service**: Business logic service implementations

### Presentation Layer
- **ProjectTemplate.Presentation**: API controllers
- **ProjectTemplate.API**: Web API host and configuration

## 🎯 Business Process Automation Features

### Core Entities

1. **Workflow** - Defines business process templates
   - Id (Guid, PK)
   - Name, Description, Version
   - IsActive flag
   - Collection of WorkflowSteps

2. **WorkflowStep** - Individual steps in a workflow
   - Id (Guid, PK), WorkflowId (FK)
   - StepName, Order, ResponsibleRole
   - DueInHours (optional)

3. **Request** - Actual business process instances
   - Id (Guid, PK), Type (enum), InitiatorId (FK)
   - Status (enum), Title, Description
   - Collection of RequestSteps

4. **RequestStep** - Tracks progress through workflow steps
   - Id (Guid, PK), RequestId (FK), WorkflowStepId (FK)
   - Status, ValidatedAt, ValidatorId, Comments

5. **Notification** - User notifications
   - Id (Guid, PK), UserId (FK)
   - Message, IsRead, Type, ActionUrl

### Enums
- **RequestType**: Leave, Expense, Training, ITSupport, ProfileUpdate
- **RequestStatus**: Pending, Approved, Rejected, Archived
- **StepStatus**: Pending, Approved, Rejected

## 🔧 API Endpoints

### Workflow Management
- `GET /api/workflow` - Get all workflows
- `GET /api/workflow/active` - Get active workflows only
- `GET /api/workflow/{id}` - Get workflow by ID
- `GET /api/workflow/{id}/steps` - Get workflow with steps
- `POST /api/workflow` - Create new workflow with steps
- `PUT /api/workflow/{id}` - Update workflow
- `DELETE /api/workflow/{id}` - Delete workflow
- `PATCH /api/workflow/{id}/activate` - Activate workflow
- `PATCH /api/workflow/{id}/deactivate` - Deactivate workflow

### Request Management
- `GET /api/request` - Get all requests
- `GET /api/request/{id}` - Get request by ID
- `GET /api/request/{id}/steps` - Get request with steps
- `GET /api/request/my-requests` - Get current user's requests
- `GET /api/request/status/{status}` - Get requests by status
- `GET /api/request/pending-approvals` - Get pending approvals for user
- `POST /api/request` - Create new request
- `PUT /api/request/{id}` - Update request
- `DELETE /api/request/{id}` - Delete request
- `POST /api/request/{requestId}/steps/{stepId}/approve` - Approve step
- `POST /api/request/{requestId}/steps/{stepId}/reject` - Reject step

### Notification Management
- `GET /api/notification` - Get all notifications
- `GET /api/notification/{id}` - Get notification by ID
- `GET /api/notification/my-notifications` - Get user notifications
- `GET /api/notification/unread` - Get unread notifications
- `GET /api/notification/unread-count` - Get unread count
- `POST /api/notification` - Create notification
- `PUT /api/notification/{id}` - Update notification
- `DELETE /api/notification/{id}` - Delete notification
- `PATCH /api/notification/{id}/mark-read` - Mark as read
- `PATCH /api/notification/mark-all-read` - Mark all as read

## 🚀 Getting Started

### Prerequisites
- .NET 8 SDK
- SQLite (for development)

### Setup
1. Clone the repository
2. Navigate to the project directory
3. Run database migrations:
   ```bash
   dotnet ef database update --project ProjectTemplate.Models --startup-project ProjectTemplate.API
   ```
4. Start the API:
   ```bash
   dotnet run --project ProjectTemplate.API
   ```
5. Open Swagger UI: http://localhost:5000/swagger

### Testing
Use the provided `test-endpoints.http` file to test the API endpoints.

## 🔒 Security Features
- JWT Bearer token authentication
- Identity Framework integration
- CORS configuration
- Request validation
- Role-based authorization

## 📊 Database
- Uses SQLite for development (easily switchable to SQL Server)
- Entity Framework Core Code First approach
- Automatic migrations
- Optimized indexes for performance

## 🎨 Key Design Patterns
- Repository Pattern
- Service Layer Pattern
- Dependency Injection
- AutoMapper for object mapping
- Clean Architecture separation

## 📝 Business Process Flow
1. **Create Workflow**: Define process template with steps
2. **Submit Request**: User creates request based on workflow
3. **Process Steps**: Request moves through workflow steps
4. **Approval/Rejection**: Authorized users approve/reject steps
5. **Notifications**: Users receive notifications about status changes
6. **Completion**: Request reaches final status

This platform provides a solid foundation for building complex business process automation systems with proper separation of concerns and scalable architecture.

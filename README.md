# Project Template - Clean Architecture .NET 8 Web API

This is a template project based on Clean Architecture principles for .NET 8 Web API applications. It provides a solid foundation for building scalable and maintainable web APIs.

## 🏗️ Architecture Overview

The solution follows Clean Architecture principles with the following layers:

### Core Layer
- **ProjectTemplate.Models**: Contains entities, data context, and domain models
- **ProjectTemplate.Contracts**: Contains interfaces and contracts
- **ProjectTemplate.Shared**: Contains DTOs, request parameters, and shared models

### Infrastructure Layer
- **ProjectTemplate.Repository**: Contains data access implementations
- **ProjectTemplate.LoggerService**: Contains logging service implementation

### Application Layer
- **ProjectTemplate.Service.Contracts**: Contains service interfaces
- **ProjectTemplate.Service**: Contains business logic implementations

### Presentation Layer
- **ProjectTemplate.Presentation**: Contains controllers
- **ProjectTemplate.API**: Main API project with configuration and startup

## 🚀 Getting Started

### Prerequisites
- .NET 8 SDK
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 or VS Code

### Quick Setup

1. **Copy the template**
   ```bash
   # Copy the ProjectTemplate folder to your desired location
   cp -r ProjectTemplate YourProjectName
   cd YourProjectName
   ```

2. **Run the setup script** (Recommended)
   ```bash
   # Use the provided PowerShell script to rename everything
   .\setup-new-project.ps1 -ProjectName "YourProjectName"
   ```

   Or manually:
   - Rename all `ProjectTemplate` references to your project name
   - Update namespaces in all files
   - Update project references in .csproj files

3. **Configure Database Connection**
   - Update connection string in `appsettings.json` and `appsettings.Development.json`
   - Replace `YOUR_SERVER` and `YOUR_DATABASE` with your actual values

4. **Configure JWT Settings**
   - Update JWT settings in `appsettings.json`
   - Replace `YOUR_JWT_SECRET_KEY_HERE_MAKE_IT_LONG_AND_SECURE` with a secure key

5. **Install Dependencies**
   ```bash
   dotnet restore
   ```

6. **Run Database Migrations**
   ```bash
   dotnet ef database update --project ProjectTemplate.API
   ```

7. **Run the Application**
   ```bash
   dotnet run --project ProjectTemplate.API
   ```

## 📁 Project Structure

```
ProjectTemplate/
├── ProjectTemplate.API/                 # Main API project
│   ├── Controllers/                     # API controllers (if any)
│   ├── ExtensionMethods/               # Service configuration extensions
│   ├── wwwroot/                        # Static files
│   ├── Program.cs                      # Application entry point
│   ├── appsettings.json               # Configuration
│   └── nlog.config                    # Logging configuration
├── ProjectTemplate.Contracts/           # Interfaces and contracts
│   └── Repository/                     # Repository interfaces
├── ProjectTemplate.LoggerService/       # Logging implementation
├── ProjectTemplate.Models/              # Domain models and data context
│   ├── DataSource/                     # DbContext
│   ├── Entities/                       # Domain entities
│   ├── ErrorModel/                     # Error handling models
│   └── Exceptions/                     # Custom exceptions
├── ProjectTemplate.Presentation/        # Controllers
│   └── Controllers/                    # API controllers
├── ProjectTemplate.Repository/          # Data access layer
│   └── Repository/                     # Repository implementations
├── ProjectTemplate.Service/             # Business logic layer
├── ProjectTemplate.Service.Contracts/   # Service interfaces
└── ProjectTemplate.Shared/              # Shared models and DTOs
    ├── DataTransferObjects/            # DTOs
    └── RequestFeatures/               # Request parameters
```

## 🔧 Key Features

- **Clean Architecture**: Separation of concerns with clear layer boundaries
- **Entity Framework Core**: ORM for data access
- **JWT Authentication**: Token-based authentication
- **AutoMapper**: Object-to-object mapping
- **NLog**: Structured logging
- **Swagger/OpenAPI**: API documentation
- **CORS**: Cross-origin resource sharing support
- **Exception Handling**: Global exception handling middleware
- **Repository Pattern**: Data access abstraction
- **Service Layer**: Business logic separation

## 📝 How to Use This Template

### Adding New Entities

1. Create entity in `ProjectTemplate.Models/Entities/`
2. Add DbSet to `ProjectTemplateContext`
3. Create repository interface in `ProjectTemplate.Contracts/Repository/`
4. Implement repository in `ProjectTemplate.Repository/Repository/`
5. Add repository to `RepositoryManager`

### Adding New Services

1. Create service interface in `ProjectTemplate.Service.Contracts/`
2. Implement service in `ProjectTemplate.Service/`
3. Add service to `ServiceManager`
4. Register service in `ServiceExtensions.cs`

### Adding New Controllers

1. Create controller in `ProjectTemplate.Presentation/Controllers/`
2. Inherit from `BaseApiController`
3. Inject required services through constructor

## 🔒 Security Features

- JWT Bearer token authentication
- CORS configuration
- HTTPS redirection
- Request size limits
- Model validation

## 📊 Logging

The template uses NLog for logging with the following levels:
- Debug
- Info
- Warn
- Error

Logs are written to files in the `logs/` directory.

## 🧪 Testing

To add tests to your project:
1. Create test projects for each layer
2. Use xUnit, NUnit, or MSTest
3. Mock dependencies using Moq or similar

## 📚 Additional Resources

- [Clean Architecture by Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [.NET 8 Documentation](https://docs.microsoft.com/en-us/dotnet/)
- [Entity Framework Core Documentation](https://docs.microsoft.com/en-us/ef/core/)

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## 📄 License

This template is provided as-is for educational and development purposes.

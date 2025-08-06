# Configuration Guide

This document explains how to configure your new project after using the template.

## 🔧 Required Configurations

### 1. Database Connection String

Update the connection string in `appsettings.json` and `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "sqlConnection": "Server=YOUR_SERVER;Database=YOUR_DATABASE;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;"
  }
}
```

**Common Connection String Examples:**

- **SQL Server Express LocalDB:**
  ```
  Server=(localdb)\\mssqllocaldb;Database=YourProjectDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;
  ```

- **SQL Server with Windows Authentication:**
  ```
  Server=.\\SQLEXPRESS;Database=YourProjectDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;
  ```

- **SQL Server with SQL Authentication:**
  ```
  Server=YOUR_SERVER;Database=YourProjectDB;User Id=YOUR_USERNAME;Password=YOUR_PASSWORD;MultipleActiveResultSets=true;TrustServerCertificate=True;
  ```

### 2. JWT Configuration

Update JWT settings in `appsettings.json`:

```json
{
  "JWT": {
    "ValidAudience": "https://localhost:5001;http://localhost:5000",
    "ValidIssuer": "https://localhost:5001;http://localhost:5000",
    "Secret": "YOUR_VERY_LONG_AND_SECURE_SECRET_KEY_AT_LEAST_32_CHARACTERS"
  }
}
```

**Important:** 
- The secret key must be at least 32 characters long
- Use a cryptographically secure random string
- Never commit real secrets to version control

### 3. Email Configuration (Optional)

If you plan to use email services, update:

```json
{
  "EmailConfiguration": {
    "From": "your-email@example.com",
    "SmtpServer": "smtp.gmail.com",
    "Port": 465,
    "Username": "your-email@example.com",
    "Password": "your-app-password"
  }
}
```

### 4. CORS Configuration

Update CORS origins in `Program.cs` and `ServiceExtensions.cs`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder
            .WithOrigins("http://localhost:4200", "https://yourdomain.com") // Add your frontend URLs
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});
```

## 🗃️ Database Setup

### 1. Install Entity Framework Tools

```bash
dotnet tool install --global dotnet-ef
```

### 2. Create Initial Migration

```bash
dotnet ef migrations add InitialCreate --project YourProject.API
```

### 3. Update Database

```bash
dotnet ef database update --project YourProject.API
```

## 🔐 Security Best Practices

### Environment Variables

For production, use environment variables instead of hardcoded values:

```json
{
  "ConnectionStrings": {
    "sqlConnection": "${CONNECTION_STRING}"
  },
  "JWT": {
    "Secret": "${JWT_SECRET}"
  }
}
```

### User Secrets (Development)

For development, use user secrets:

```bash
dotnet user-secrets init --project YourProject.API
dotnet user-secrets set "JWT:Secret" "your-development-secret" --project YourProject.API
dotnet user-secrets set "ConnectionStrings:sqlConnection" "your-dev-connection-string" --project YourProject.API
```

## 📝 Logging Configuration

The template uses NLog. Configuration is in `nlog.config`:

```xml
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd">
    <targets>
        <target name="logfile" xsi:type="File"
        fileName="logs\${shortdate}_logfile.txt"
        layout="${longdate} ${level:uppercase=true} ${message}"/>
    </targets>
    <rules>
        <logger name="*" minlevel="Debug" writeTo="logfile" />
    </rules>
</nlog>
```

## 🚀 Deployment Configuration

### Production appsettings.Production.json

Create `appsettings.Production.json`:

```json
{
  "ConnectionStrings": {
    "sqlConnection": "${CONNECTION_STRING}"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "JWT": {
    "ValidAudience": "https://yourproductiondomain.com",
    "ValidIssuer": "https://yourproductiondomain.com",
    "Secret": "${JWT_SECRET}"
  }
}
```

### Docker Configuration (Optional)

If using Docker, create `Dockerfile`:

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["YourProject.API/YourProject.API.csproj", "YourProject.API/"]
# Copy other project files...
RUN dotnet restore "YourProject.API/YourProject.API.csproj"
COPY . .
WORKDIR "/src/YourProject.API"
RUN dotnet build "YourProject.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "YourProject.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "YourProject.API.dll"]
```

## ✅ Verification Checklist

After configuration, verify:

- [ ] Database connection works
- [ ] JWT authentication is configured
- [ ] CORS allows your frontend origins
- [ ] Logging is working (check logs folder)
- [ ] Swagger UI is accessible at `/swagger`
- [ ] API endpoints respond correctly
- [ ] Email service works (if configured)

## 🆘 Troubleshooting

### Common Issues

1. **Database Connection Failed**
   - Check connection string format
   - Verify SQL Server is running
   - Check firewall settings

2. **JWT Token Invalid**
   - Verify secret key length (minimum 32 characters)
   - Check issuer and audience settings
   - Ensure clock synchronization

3. **CORS Errors**
   - Add your frontend URL to CORS origins
   - Check for trailing slashes in URLs
   - Verify HTTPS/HTTP protocol matches

4. **Migration Errors**
   - Ensure Entity Framework tools are installed
   - Check database permissions
   - Verify connection string in design time

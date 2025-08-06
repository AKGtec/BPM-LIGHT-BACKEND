using Microsoft.EntityFrameworkCore;
using ProjectTemplate.Contracts;
using ProjectTemplate.Contracts.Repository;
using ProjectTemplate.LoggerService;
using ProjectTemplate.Models.DataSource;
using ProjectTemplate.Repository.Repository;
using ProjectTemplate.Service;
using ProjectTemplate.Service.Contracts;

namespace ProjectTemplate.API.ExtensionMethods;

public static class ServiceExtensions
{
    public static void ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
            builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
        });
    }

    public static void ConfigureIISIntegration(this IServiceCollection services)
    {
        services.Configure<IISOptions>(options =>
        {
        });
    }

    public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ProjectTemplateContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("sqlConnection")));
        services.AddScoped<ProjectTemplateContext>();
    }

    public static void ConfigureLoggerService(this IServiceCollection services)
    {
        services.AddSingleton<ILoggerManager, LoggerManager>();
    }

    public static void ConfigureRepositoryManager(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IRepositoryManager, RepositoryManager>();

        // Add specific repository services
        services.AddScoped<IWorkflowRepository, WorkflowRepository>();
        services.AddScoped<IRequestRepository, RequestRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
    }

    public static void ConfigureServiceManager(this IServiceCollection services)
    {
        services.AddScoped<IServiceManager, ServiceManager>();

        // Add specific services
        services.AddScoped<IWorkflowService, WorkflowService>();
        services.AddScoped<IRequestService, RequestService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
    }

    public static void ConfigureAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingProfile));
    }
}

using ClassroomSync.Application.Interfaces.Client;
using ClassroomSync.Application.Interfaces.Repositories;
using ClassroomSync.Infrastructure.Clients;
using ClassroomSync.Infrastructure.Data.Context;
using ClassroomSync.Infrastructure.Repositories;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Classroom.v1;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Util.Store;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClassroomSync.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddDbConnection(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Default")));
        
        return services;
    }

    public static IServiceCollection AddSheetServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<SheetsService>(provider =>
        {
            var credentialsPath = configuration["GoogleApi:CredentialsPath"];
            var applicationName = configuration["GoogleApi:ApplicationName"];

            GoogleCredential credential;
            using (var stream = new FileStream(credentialsPath!, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped(SheetsService.Scope.Spreadsheets);
            }

            return new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = applicationName
            });
        });
        
        return services;
    }
    
    public static IServiceCollection AddGoogleAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        string secretPath = configuration["GoogleApi:ClientSecretPath"] 
                            ?? throw new InvalidOperationException("Шлях до client_secret не знайдено в налаштуваннях.");

        ClientSecrets secrets;
        using (var stream = new FileStream(secretPath, FileMode.Open, FileAccess.Read))
        {
            secrets = GoogleClientSecrets.Load(stream).Secrets;
        }

        services.AddSingleton(secrets);
        
        return services;
    }

    public static IServiceCollection AddGoogleClients(this IServiceCollection services)
    {
        services.AddScoped<IGoogleSheetClient, GoogleSheetsClient>();
        services.AddScoped<IGoogleAuthClient, GoogleAuthClient>();
        services.AddScoped<IGoogleClassroomClient, GoogleClassroomClient>();
        
        return services;   
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<IAssignmentRepository, AssignmentRepository>();
        services.AddScoped<ISubmissionRepository, SubmissionsRepository>();
        
        return services;
    }
}
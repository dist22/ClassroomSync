using System.Text;
using ClassroomSync.Application.Interfaces.Client;
using ClassroomSync.Application.Interfaces.Jobs;
using ClassroomSync.Application.Interfaces.Repositories;
using ClassroomSync.Infrastructure.Clients;
using ClassroomSync.Infrastructure.Data.Context;
using ClassroomSync.Infrastructure.Jobs;
using ClassroomSync.Infrastructure.Repositories;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Classroom.v1;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Util.Store;
using Hangfire;
using Hangfire.PostgreSql;
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
        ClientSecrets secrets;

        // 1. Спочатку шукаємо JSON текст зі змінних середовища (для хмари)
        string secretJson = configuration["GoogleApi:ClientSecretJson"];

        if (!string.IsNullOrEmpty(secretJson))
        {
            // Перетворюємо рядок на потік байтів, щоб Google SDK зміг його прочитати
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(secretJson)))
            {
                secrets = GoogleClientSecrets.Load(stream).Secrets;
            }
        }
        else
        {
            // 2. Якщо тексту немає, шукаємо файл на диску (для локальної розробки)
            string secretPath = configuration["GoogleApi:ClientSecretPath"] 
                                ?? throw new InvalidOperationException("Ані ClientSecretJson, ані ClientSecretPath не знайдено в налаштуваннях.");

            using (var stream = new FileStream(secretPath, FileMode.Open, FileAccess.Read))
            {
                secrets = GoogleClientSecrets.Load(stream).Secrets;
            }
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

    public static IServiceCollection AddHangFire(this IServiceCollection services ,IConfiguration configuration)
    {
        
        services.AddScoped<IJobDispatcher, HangfireJobDispatcher>();

        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(configuration.GetConnectionString("Default"))); 

        services.AddHangfireServer();
        
        return services;
    }
}
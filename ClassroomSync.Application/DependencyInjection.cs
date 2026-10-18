using System.Reflection;
using ClassroomSync.Application.Interfaces.Sevices;
using ClassroomSync.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ClassroomSync.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICourseServiсe, CourseService>();
        services.AddScoped<IAssignmentService, AssignmentService>();
        services.AddScoped<ISubmissionService, SubmissionService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IReportService, ReportService>();
        
        return services;
    }

    public static IServiceCollection AddAutoMapper(this IServiceCollection services)
    {

        services.AddAutoMapper(cfg => { },Assembly.GetExecutingAssembly());
        
        return services;
    }
}
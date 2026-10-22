using System.Reflection;
using ClassroomSync.Application.Interfaces.Jobs;
using ClassroomSync.Application.Interfaces.Sevices;
using ClassroomSync.Application.Jobs;
using ClassroomSync.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ClassroomSync.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<IAssignmentService, AssignmentService>();
        services.AddScoped<ISubmissionService, SubmissionService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IReportService, ReportService>();
        
        return services;
    }

    public static IServiceCollection AddWorkflowJob(this IServiceCollection services)
    {
        services.AddScoped<IClassroomWorkflowJob, ClassroomWorkflowJob>();
        services.AddScoped<IReportWorkflowJob, ReportWorkflowJob>();
        
        return services;
    }

    public static IServiceCollection AddAutoMapper(this IServiceCollection services)
    {

        services.AddAutoMapper(cfg => { },Assembly.GetExecutingAssembly());
        
        return services;
    }
}
namespace ClassroomSync.WebApi.Extensions;

public static class HealthCheckEndpoint
{
    public static void MapHealthCheck(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/health", () => Results.Ok("Backend online!"))
            .WithName("HealthCheck")
            .WithTags("System");
    }
}
using ClassroomSync.Application.Interfaces.Sevices;
using Hangfire;

namespace ClassroomSync.WebApi.Extensions;

public static class UserRecurringJobs
{
    public static void UseRecurringJobs(this IApplicationBuilder app)
    {
        
        var kyivTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Kyiv");
        
        RecurringJob.AddOrUpdate<ISyncService>(
            recurringJobId: "nightly-mass-sync",
            methodCall: service => service.TriggerAllUsersSyncAsync(CancellationToken.None),
            cronExpression: "0 3 * * *",
            options: new RecurringJobOptions
            {
                TimeZone = kyivTimeZone
            }
        );

        RecurringJob.AddOrUpdate<ISyncService>(
            recurringJobId: "nightly-sheet-sync",
            methodCall: service => service.TriggerMasterSheetSync(),
            cronExpression: "5 3 * * *",
            options: new RecurringJobOptions
            {
                TimeZone = kyivTimeZone
            }
        );
    }
}
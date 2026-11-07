using ClassroomSync.Application.Interfaces.Sevices;
using Hangfire;

namespace ClassroomSync.WebApi.Extensions;

public static class UserRecurringJobs
{
    public static void UseRecurringJobs(this IApplicationBuilder app)
    {
        var kyivTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Kyiv");

        RecurringJob.AddOrUpdate<ISyncService>(
            recurringJobId: "hourly-mass-sync",
            methodCall: service => service.TriggerAllUsersSyncAsync(CancellationToken.None),
            cronExpression: Cron.Hourly,
            options: new RecurringJobOptions
            {
                TimeZone = kyivTimeZone
            }
        );

        RecurringJob.AddOrUpdate<ISyncService>(
            recurringJobId: "nightly-sheet-sync",
            methodCall: service => service.TriggerMasterSheetSync(TimeSpan.FromMinutes(2)),
            cronExpression: Cron.Hourly,
            options: new RecurringJobOptions
            {
                TimeZone = kyivTimeZone
            }
        );
        
    }
}
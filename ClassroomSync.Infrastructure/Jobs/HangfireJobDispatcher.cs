using ClassroomSync.Application.Interfaces.Jobs;
using ClassroomSync.Application.Interfaces.Sevices;
using ClassroomSync.Application.Services;
using Hangfire;

namespace ClassroomSync.Infrastructure.Jobs;

public class HangfireJobDispatcher(IBackgroundJobClient backgroundJobClient) : IJobDispatcher
{
    public void EnqueueClassroomSync(Guid userId)
    {
        var step1Id = backgroundJobClient.Enqueue<IClassroomWorkflowJob>(job =>
            job.SyncCoursesAsync(userId, CancellationToken.None));
        
        backgroundJobClient.ContinueJobWith<IClassroomWorkflowJob>(step1Id, job =>
            job.SyncAssignmentsAndSubmissionsAsync(userId, CancellationToken.None));
    }

    public void ScheduleSheetUpdate(TimeSpan daley) => backgroundJobClient.Schedule<IReportWorkflowJob>(job => job.UpdateGoogleSheetAsync(CancellationToken.None), daley);
    public void EnqueueCourseSubscription(string googleCourseId, string refreshToken) => backgroundJobClient.Enqueue<ICourseSubscriptionService>(job => job.SubscribeToCourseEventsAsync(googleCourseId, refreshToken,CancellationToken.None));
}
namespace ClassroomSync.Application.Interfaces.Sevices;

public interface ICourseSubscriptionService
{
    public Task SubscribeToCourseEventsAsync (string googleCourseId, CancellationToken cancellationToken);
}
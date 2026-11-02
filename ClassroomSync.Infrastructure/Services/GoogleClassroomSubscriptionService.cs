using ClassroomSync.Application.Interfaces.Sevices;
using ClassroomSync.Application.Services;
using ClassroomSync.Infrastructure.Configuration;
using Google;
using Google.Apis.Classroom.v1;
using Google.Apis.Classroom.v1.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ClassroomSync.Infrastructure.Services;

public class GoogleClassroomSubscriptionService(ClassroomService classroomService, ILogger<GoogleClassroomSubscriptionService> logger, IOptions<GoogleCloudOptions> googleCloudOptions) : ICourseSubscriptionService
{
    public async Task SubscribeToCourseEventsAsync(string googleCourseId, CancellationToken cancellationToken)
    {
        var topicName = googleCloudOptions.Value.PubSubTopicName;
        
        var feedsToRegister = new[]
        {
            new Feed 
            { 
                FeedType = "COURSE_WORK_CHANGES", 
                CourseWorkChangesInfo = new CourseWorkChangesInfo { CourseId = googleCourseId } 
            },
        };

        foreach (var feed in feedsToRegister)
        {
            var registration = new Registration
            {
                Feed = feed,
                CloudPubsubTopic = new CloudPubsubTopic { TopicName = topicName }
            };

            try
            {
                await classroomService.Registrations.Create(registration).ExecuteAsync(cancellationToken);
                logger.LogInformation("Успішно оформлено підписку {FeedType} для курсу {CourseId}", feed.FeedType, googleCourseId);
            }
            catch (GoogleApiException ex) when (ex.HttpStatusCode == System.Net.HttpStatusCode.Conflict)
            {
                // 409 Conflict - підписка вже існує (наприклад, для іншого студента в цьому ж курсі). Все ок.
                logger.LogDebug("Підписка на {FeedType} для курсу {CourseId} вже існує.", feed.FeedType, googleCourseId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Помилка при реєстрації вебхука для курсу {CourseId}", googleCourseId);
            }
        }
    }
}
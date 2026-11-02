using System.Text;
using System.Text.Json;
using ClassroomSync.Application.Interfaces.Repositories;
using ClassroomSync.Application.Interfaces.Sevices;
using ClassroomSync.Application.Models.Webhooks;
using Microsoft.Extensions.Logging;

namespace ClassroomSync.Application.Services;

public class WebhookService(ISyncService syncService, ILogger logger, ICourseRepository courseRepository) : IWebhookService
{
    public async Task ProcessGoogleClassroomNotification(PubSubPayload payload)
    {
        if (string.IsNullOrEmpty(payload?.Message?.Data))
        {
            logger.LogWarning("Отримано порожній вебхук від Google Pub/Sub.");
            return;
        }

        try
        {
            var base64Bytes = Convert.FromBase64String(payload.Message.Data);
            var decodedJson = Encoding.UTF8.GetString(base64Bytes);
            
            var notification = JsonSerializer.Deserialize<ClassroomNotification>(decodedJson);

            logger.LogInformation("Отримано вебхук для курсу: {CourseId}, Подія: {EventType}", 
                notification?.ResourceId?.CourseId, notification?.EventType);

            if (notification?.ResourceId?.CourseId != null)
            {
                var googleCourseId =  notification.ResourceId.CourseId;
                var userIds = await courseRepository.GetUserIdsByGoogleCourseIdAsync(googleCourseId, CancellationToken.None);

                if (userIds.Count != 0)
                {
                    logger.LogWarning("Отримано вебхук для курсу {CourseId}, але жоден користувач у базі його не відстежує.", googleCourseId);
                    return;
                }
                foreach (var userId in userIds) syncService.TriggerUserSync(userId, CancellationToken.None);
                
                syncService.TriggerMasterSheetSync(TimeSpan.FromMinutes(2)); 
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Помилка обробки даних вебхука від Classroom");
        }
    }
    
}
using ClassroomSync.Application.Interfaces.Client;
using ClassroomSync.Application.Interfaces.Repositories;
using ClassroomSync.Application.Interfaces.Sevices;
using ClassroomSync.Application.Services;
using ClassroomSync.Infrastructure.Configuration;
using Google;
using Google.Apis.Classroom.v1;
using Google.Apis.Classroom.v1.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ClassroomSync.Infrastructure.Services;

public class GoogleClassroomSubscriptionService(IGoogleClassroomClient classroomClient, ILogger<GoogleClassroomSubscriptionService> logger, IUserRepository userRepository) : ICourseSubscriptionService
{
    public async Task SubscribeToCourseEventsAsync(string googleCourseId, string refreshToken,CancellationToken cancellationToken)
    {
        try
        {
            // Викликаємо метод нашого клієнта
            await classroomClient.RegisterCourseWebhookAsync(googleCourseId, refreshToken, cancellationToken);
            logger.LogInformation("Успішно оформлено підписку COURSE_WORK_CHANGES для курсу {CourseId}", googleCourseId);
        }
        catch (GoogleApiException ex) when (ex.HttpStatusCode == System.Net.HttpStatusCode.Conflict)
        {
            // 409 Conflict - підписка вже існує. Це нормально, просто ігноруємо.
            logger.LogDebug("Підписка на COURSE_WORK_CHANGES для курсу {CourseId} вже існує.", googleCourseId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Помилка при реєстрації вебхука для курсу {CourseId}", googleCourseId);
            throw;
        }
    }
}
using ClassroomSync.Application.Models.Webhooks;

namespace ClassroomSync.Application.Interfaces.Sevices;

public interface IWebhookService
{
    Task ProcessGoogleClassroomNotification(PubSubPayload payload);
}
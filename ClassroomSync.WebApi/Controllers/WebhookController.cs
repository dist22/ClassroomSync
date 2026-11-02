using ClassroomSync.Application.Interfaces.Sevices;
using ClassroomSync.Application.Models.Webhooks;
using Microsoft.AspNetCore.Mvc;

namespace ClassroomSync.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class WebhookController(IWebhookService webhookService) : ControllerBase
{
    [HttpPost("google")]
    public IActionResult ReceiveGoogleClassroomPush([FromBody] PubSubPayload payload)
    {
        webhookService.ProcessGoogleClassroomNotification(payload);
        return Ok();
    }
}
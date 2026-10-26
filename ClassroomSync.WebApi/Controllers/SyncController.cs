using ClassroomSync.Application.Interfaces.Sevices;
using Microsoft.AspNetCore.Mvc;

namespace ClassroomSync.WebApi.Controllers;


[ApiController]
[Route("api/[controller]")]
public class SyncController(ISyncService syncService) : ControllerBase
{
    [HttpPost("sync-all")]
    public async Task<IActionResult> SyncAllUser(CancellationToken cancellationToken)
    {
        await syncService.TriggerAllUsersSyncAsync(cancellationToken);
        return Accepted(new {Message = "Запущено синхронізацію"});
    }
}
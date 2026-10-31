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

    [HttpPost("{userId:guid}")]
    public IActionResult SyncUser(Guid userId, CancellationToken cancellationToken)
    {
        syncService.TriggerUserSync(userId, cancellationToken);
        return Accepted(new {Message = "Синхронізацію одного користувача запущено"});
    }
    
    [HttpPost("sync-master-sheet")]
    public IActionResult SyncMasterSheet(CancellationToken cancellationToken)
    {
        syncService.TriggerMasterSheetSync();
        return Accepted(new { Message = "Загальну таблицю успішно оновлено! Кожен курс винесено на окремий аркуш." });
    }
}
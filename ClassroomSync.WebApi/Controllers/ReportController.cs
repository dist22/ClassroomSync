using ClassroomSync.Application.Interfaces.Sevices;
using Microsoft.AspNetCore.Mvc;

namespace ClassroomSync.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportController(IReportService reportService) : ControllerBase
{
    [HttpPost("sync-master-sheet")]
    public async Task<IActionResult> SyncMasterSheet(CancellationToken cancellationToken)
    {
        await reportService.SyncMasterReportAsync(cancellationToken);
        return Ok(new { Message = "Загальну таблицю успішно оновлено! Кожен курс винесено на окремий аркуш." });
    }
}
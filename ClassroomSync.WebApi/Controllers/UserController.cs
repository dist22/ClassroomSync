using ClassroomSync.Application.DTOs;
using ClassroomSync.Application.Interfaces.Sevices;
using Microsoft.AspNetCore.Mvc;

namespace ClassroomSync.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllUsersAsync(CancellationToken cancellationToken)
    {
        var user = await userService.GetAllUsersAsync(cancellationToken);
        return Ok(user);
    }
}
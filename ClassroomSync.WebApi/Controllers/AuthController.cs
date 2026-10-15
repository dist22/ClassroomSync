using ClassroomSync.Application.Interfaces.Sevices;
using Microsoft.AspNetCore.Mvc;

namespace ClassroomSync.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpGet("login")]
    public IActionResult Login()
    {
        // Динамічно генеруємо URL нашого ендпоінту Callback, щоб він збігався з налаштуваннями в Google Cloud
        var redirectUri = Url.Action(nameof(Callback), "Auth", null, Request.Scheme);
        
        if (string.IsNullOrEmpty(redirectUri))
        {
            return BadRequest("Не вдалося згенерувати Redirect URI.");
        }

        var googleLoginUrl = authService.GetLoginUrl(redirectUri);
        
        // Відправляємо користувача на сторінку логіну Google
        return Redirect(googleLoginUrl);
    }

    [HttpGet("callback")]
    public async Task<IActionResult> Callback([FromQuery] string code, CancellationToken cancellationToken)
    {
        // Якщо користувач скасував авторизацію, Google не поверне code
        if (string.IsNullOrEmpty(code))
        {
            return BadRequest("Код авторизації відсутній. Можливо, ви скасували вхід.");
        }

        try
        {
            var redirectUri = Url.Action(nameof(Callback), "Auth", null, Request.Scheme);
            
            // Передаємо code, redirectUri та CancellationToken у наш бізнес-сервіс
            var userId = await authService.ProcessGoogleCallbackAsync(code, redirectUri!, cancellationToken);

            // Тимчасове рішення для тестування. 
            // У майбутньому тут можна буде генерувати JWT-токен вашої системи для фронтенду.
            return Ok(new 
            { 
                Message = "Авторизація успішна! Профіль створено або оновлено.", 
                UserId = userId 
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = ex.Message });
        }
    }
}
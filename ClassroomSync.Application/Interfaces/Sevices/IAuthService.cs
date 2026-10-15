namespace ClassroomSync.Application.Interfaces.Sevices;

public interface IAuthService
{
    public string GetLoginUrl(string redirectUri);
    public Task<Guid> ProcessGoogleCallbackAsync(string code, string redirectUri, CancellationToken cancellationToken = default);
}
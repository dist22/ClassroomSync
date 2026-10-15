using ClassroomSync.Application.DTOs;

namespace ClassroomSync.Application.Interfaces.Client;

public interface IGoogleAuthClient
{
    public string GetAuthorizationUrl(string redirectUri);
    public Task<GoogleUserInfoDto> ExchangeCodeForTokenAsync(string code, string redirectUri, CancellationToken cancellationToken = default);
}
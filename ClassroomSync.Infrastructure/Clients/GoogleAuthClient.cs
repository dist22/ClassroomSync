using System.Net.Http.Headers;
using System.Text.Json;
using ClassroomSync.Application.DTOs;
using ClassroomSync.Application.Interfaces.Client;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Classroom.v1;

namespace ClassroomSync.Infrastructure.Clients;

public class GoogleAuthClient(ClientSecrets clientSecrets) : IGoogleAuthClient
{
    private GoogleAuthorizationCodeFlow GetFlow()
    {
        return new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = clientSecrets,
            Scopes = new[]
            {
                ClassroomService.Scope.ClassroomCoursesReadonly,
                "https://www.googleapis.com/auth/userinfo.email",
                "https://www.googleapis.com/auth/userinfo.profile",
                ClassroomService.Scope.ClassroomCourseworkMeReadonly
            }
        });
    }

    public string GetAuthorizationUrl(string redirectUri)
    {
        var flow = GetFlow();

        // Приводимо базовий інтерфейс до специфічного класу Google
        var request =
            (Google.Apis.Auth.OAuth2.Requests.GoogleAuthorizationCodeRequestUrl)flow.CreateAuthorizationCodeRequest(
                redirectUri);

        // Тепер властивості доступні, і бібліотека сама згенерує правильний URL без дублювання
        request.AccessType = "offline";
        request.Prompt = "consent";

        return request.Build().ToString();
    }

    public async Task<GoogleUserInfoDto> ExchangeCodeForTokenAsync(string code, string redirectUri,
        CancellationToken cancellationToken)
    {
        var flow = GetFlow();

        var tokenResponse = await flow.ExchangeCodeForTokenAsync("user", code, redirectUri, cancellationToken);

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

        var userInfoResponse = await httpClient.GetStringAsync("https://www.googleapis.com/oauth2/v2/userinfo");
        using var jsonDocument = JsonDocument.Parse(userInfoResponse);
        var root = jsonDocument.RootElement;

        return new GoogleUserInfoDto(
            GoogleId: root.GetProperty("id").GetString()!,
            Email: root.GetProperty("email").GetString()!,
            FirstName: root.TryGetProperty("given_name", out var fn) ? fn.GetString()! : string.Empty,
            LastName: root.TryGetProperty("family_name", out var ln) ? ln.GetString()! : string.Empty,
            RefreshToken: tokenResponse.RefreshToken
        );
    }
}
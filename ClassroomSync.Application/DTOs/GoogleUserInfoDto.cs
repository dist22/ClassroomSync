namespace ClassroomSync.Application.DTOs;

public record GoogleUserInfoDto(
    string GoogleId,
    string Email,
    string FirstName,
    string LastName,
    string? RefreshToken);
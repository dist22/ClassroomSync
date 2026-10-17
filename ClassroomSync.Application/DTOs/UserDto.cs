namespace ClassroomSync.Application.DTOs;

public record UserDto(
    Guid Id,
    string GoogleId,
    string Email);
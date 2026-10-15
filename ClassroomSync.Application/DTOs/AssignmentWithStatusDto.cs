namespace ClassroomSync.Application.DTOs;

public record AssignmentWithStatusDto(
    Guid Id,
    string Title,
    DateTime? DateTime,
    double? MaxPoints,
    string Status,
    double? AssignedGrada
);
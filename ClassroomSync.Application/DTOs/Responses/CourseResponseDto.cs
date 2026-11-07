namespace ClassroomSync.Application.DTOs.Responses;

public record CourseResponseDto(
    Guid Id,
    string GoogleCourseId,
    string Name,
    string Url);
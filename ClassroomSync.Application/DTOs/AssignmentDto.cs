namespace ClassroomSync.Application.DTOs;

public record AssignmentDto(
    string Id,
    string CourseId, // Зв'язок із курсом
    string Title,
    DateTime? DueDate, // Дедлайн
    double? MaxPoints);
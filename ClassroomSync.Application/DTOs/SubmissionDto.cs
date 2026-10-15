namespace ClassroomSync.Application.DTOs;

public record SubmissionDto(
    string Id,
    string CourseWorkId, 
    string State,
    double? AssignedGrade);
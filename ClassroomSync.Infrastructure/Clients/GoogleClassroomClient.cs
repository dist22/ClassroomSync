using ClassroomSync.Application.DTOs;
using ClassroomSync.Application.Interfaces.Client;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Classroom.v1;
using Google.Apis.Services;
using Microsoft.Extensions.Configuration;

namespace ClassroomSync.Infrastructure.Clients;

public class GoogleClassroomClient(ClientSecrets clientSecrets, IConfiguration configuration) : IGoogleClassroomClient
{
    private readonly string _applicationName = configuration["GoogleApi:ApplicationName"] ?? "ClassroomSync";

    private ClassroomService CreateService(string refreshToken)
    {
        var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = clientSecrets
        });

        var credential = new UserCredential(flow, "user", new TokenResponse
        {
            RefreshToken = refreshToken
        });

        return new ClassroomService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = _applicationName
        });
    }

    public async Task<IEnumerable<CourseDto>> GetStudentCoursesAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        // 1. Створюємо автентифікований сервіс
        var service = CreateService(refreshToken);

        // 2. Формуємо запит до API Classroom
        var request = service.Courses.List();
        request.StudentId = "me"; // Тільки ті курси, де він є студентом
        request.CourseStates = CoursesResource.ListRequest.CourseStatesEnum.ACTIVE;

        // 3. Виконуємо запит
        var response = await request.ExecuteAsync(cancellationToken);

        if (response.Courses == null)
        {
            return Enumerable.Empty<CourseDto>();
        }

        // 4. Мапимо відповідь від Google у наш незалежний CourseDto
        return response.Courses.Select(c => new CourseDto(
            Id: c.Id,
            Name: c.Name,
            Url: c.AlternateLink
        ));
    }

    public async Task<IEnumerable<AssignmentDto>> GetAssignmentsAsync(string googleCourseId, string refreshToken, CancellationToken cancellationToken)
    {
        var service = CreateService(refreshToken);
        var request = service.Courses.CourseWork.List(googleCourseId);
        var response = await request.ExecuteAsync(cancellationToken);

        if (response.CourseWork == null) return Enumerable.Empty<AssignmentDto>();

        return response.CourseWork.Select(cw => 
        {
            // Конвертація гуглівської дати в стандартний DateTime
            DateTime? dueDate = null;
            if (cw.DueDate != null)
            {
                var year = cw.DueDate.Year ?? DateTime.UtcNow.Year;
                var month = cw.DueDate.Month ?? 1;
                var day = cw.DueDate.Day ?? 1;
            
                var hours = cw.DueTime?.Hours ?? 23;
                var minutes = cw.DueTime?.Minutes ?? 59;
            
                dueDate = new DateTime(year, month, day, hours, minutes, 0, DateTimeKind.Utc);
            }

            return new AssignmentDto(
                Id: cw.Id,
                CourseId: cw.CourseId,
                Title: cw.Title ?? "Без назви",
                DueDate: dueDate,
                MaxPoints: cw.MaxPoints
            );
        });
    }

    public async Task<IEnumerable<SubmissionDto>> GetSubmissionsAsync(string googleCourseId, string refreshToken, CancellationToken cancellationToken = default)
    {
        var service = CreateService(refreshToken);
    
        var request = service.Courses.CourseWork.StudentSubmissions.List(googleCourseId, "-");
        request.UserId = "me"; 

        var response = await request.ExecuteAsync(cancellationToken);

        if (response.StudentSubmissions == null) return Enumerable.Empty<SubmissionDto>();

        return response.StudentSubmissions.Select(s => new SubmissionDto(
            Id: s.Id,
            CourseWorkId: s.CourseWorkId,
            State: s.State ?? string.Empty,
            AssignedGrade: s.AssignedGrade
        ));
    }
}
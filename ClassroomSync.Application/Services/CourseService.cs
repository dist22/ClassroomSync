using AutoMapper;
using ClassroomSync.Application.Interfaces.Client;
using ClassroomSync.Application.Interfaces.Repositories;
using ClassroomSync.Application.Interfaces.Sevices;
using ClassroomSync.Domain.Entities;

namespace ClassroomSync.Application.Services;

public class CourseService(
    IUserRepository userRepository,
    ICourseRepository courseRepository,
    IMapper mapper,
    IUnitOfWork unitOfWork,
    IGoogleClassroomClient googleClassroomClient) : ICourseService
{
    public async Task<List<string>> SyncUserCoursesAsync(Guid userId, string refreshToken, CancellationToken cancellationToken)
    {
        var googleCourses = await googleClassroomClient.GetStudentCoursesAsync(refreshToken, cancellationToken);
        
        if (!googleCourses.Any()) return new List<string>();
        
        var googleCourseIds = googleCourses.Select(x => x.Id).ToList();
        
        var existingCourses = await courseRepository.GetByGoogleIdsAsync(googleCourseIds, cancellationToken);
        var existingCourseIds = existingCourses.Select(c => c.GoogleCourseId).ToList();
        
        var coursesToAddDtos = googleCourses.Where(c => !existingCourseIds.Contains(c.Id)).ToList();
        
        var newCourses = mapper.Map<List<Course>>(coursesToAddDtos);
        
        foreach (var newCourse in newCourses)
        {
            await courseRepository.AddAsync(newCourse, cancellationToken);
            existingCourses.Add(newCourse); 
        }

        var user = await userRepository.GetUserWithCoursesAsync(userId, cancellationToken);
        if (user == null) return new List<string>();

        var currentUserCourseIds = user.UserCourses.Select(uc => uc.CourseId).ToList();
        
        foreach (var course in existingCourses)
        {
            if (!currentUserCourseIds.Contains(course.Id))
            {
                user.UserCourses.Add(new UserCourse
                {
                    UserId = user.Id,
                    CourseId = course.Id
                });
            }
        }
        
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return googleCourseIds;
    }
}
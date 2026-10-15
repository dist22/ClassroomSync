using AutoMapper;
using ClassroomSync.Application.DTOs;
using ClassroomSync.Application.Interfaces.Client;
using ClassroomSync.Application.Interfaces.Repositories;
using ClassroomSync.Application.Interfaces.Sevices;
using ClassroomSync.Domain.Entities;

namespace ClassroomSync.Application.Services;

public class AssignmentService(
    IAssignmentRepository assignmentRepository,
    ICourseRepository courseRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IGoogleClassroomClient googleClassroomClient) : IAssignmentService
{
    public async Task SyncAssignmentsForCoursesAsync(List<string> googleCourseIds, string refreshToken,
        CancellationToken cancellationToken)
    {
        var allAssignmentsToAdd = new List<Assignment>();

        var dbCourses = await courseRepository.GetByGoogleIdsAsync(googleCourseIds, cancellationToken);
        var courseDictionary = dbCourses.ToDictionary(x => x.GoogleCourseId, x => x.Id);

        foreach (var googleCourseId in googleCourseIds)
        {
            var googleAssignments =
                await googleClassroomClient.GetAssignmentsAsync(googleCourseId, refreshToken, cancellationToken);
            if (!googleAssignments.Any()) continue;

            var googleAssignmentIds = googleAssignments.Select(assignment => assignment.Id).ToList();

            var existingAssignments =
                await assignmentRepository.GetByGoogleIdsAsync(googleAssignmentIds, cancellationToken);
            var existingIds = existingAssignments.Select(a => a.GoogleCourseWorkId).ToList();

            var newAssignmentsDtos = googleAssignments.Where(a => !existingIds.Contains(a.Id)).ToList();

            var newAssignments = mapper.Map<List<Assignment>>(newAssignmentsDtos);
            foreach (var assignment in newAssignments)
            {
                assignment.CourseId = courseDictionary[googleCourseId];
                allAssignmentsToAdd.Add(assignment);
            }
        }

        if (allAssignmentsToAdd.Any())
        {
            await assignmentRepository.AddRangeAsync(allAssignmentsToAdd, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<IEnumerable<AssignmentWithStatusDto>> GetAssignmentForCourseAsync(Guid courseId, Guid userId,
        CancellationToken cancellationToken)
    {
        var assignments =
            await assignmentRepository.GetAssignmentWhitUserSubmissionAsync(courseId, userId, cancellationToken);
        return assignments.Select(a =>
        {
            var submissions = a.Submissions.FirstOrDefault();

            return new AssignmentWithStatusDto(
                Id: a.Id,
                Title: a.Title,
                DateTime: a.DueDate,
                MaxPoints: a.MaxPoints,
                Status: submissions?.State ?? "NEW",
                AssignedGrada: submissions?.AssignedGrade);
        });
    }
}
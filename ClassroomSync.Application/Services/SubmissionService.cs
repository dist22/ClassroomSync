using AutoMapper;
using ClassroomSync.Application.Interfaces.Client;
using ClassroomSync.Application.Interfaces.Repositories;
using ClassroomSync.Application.Interfaces.Sevices;
using ClassroomSync.Domain.Entities;

namespace ClassroomSync.Application.Services;

public class SubmissionService(
    IMapper mapper,
    ISubmissionRepository submissionRepository,
    IGoogleClassroomClient googleClassroomClient,
    IAssignmentRepository assignmentRepository,
    IUnitOfWork unitOfWork) : ISubmissionService
{
    public async Task SyncSubmissionsAsync(List<string> googleCourseIds, Guid userId, string refreshToken,
        CancellationToken cancellationToken)
    {
        var allSubmissionsToAdd = new List<StudentSubmission>();
        var allSubmissionsToUpdate = new List<StudentSubmission>();

        foreach (var googleCourseId in googleCourseIds)
        {
            
            var googleSubmissions = await googleClassroomClient.GetSubmissionsAsync(googleCourseId, refreshToken, cancellationToken);
            if (!googleSubmissions.Any()) continue;
            
            var googleAssignmentIds = googleSubmissions.Select(s => s.CourseWorkId).Distinct().ToList();
            var dbAssignments = await assignmentRepository.GetByGoogleIdsAsync(googleAssignmentIds, cancellationToken);
            
            var assignmentDictionary = dbAssignments.ToDictionary(a => a.GoogleCourseWorkId, a => a.Id);
            
            var googleSubmissionIds = googleSubmissions.Select(s => s.Id).ToList();
            var existingSubmissions = await submissionRepository.GetByGoogleIdsAsync(googleSubmissionIds, cancellationToken);
            var existingSubmissionsDict = existingSubmissions.ToDictionary(s => s.GoogleSubmissionId);
            
            foreach (var googleSub in googleSubmissions)
            {
                if (!assignmentDictionary.TryGetValue(googleSub.CourseWorkId, out var assignmentId)) continue;

                if (existingSubmissionsDict.TryGetValue(googleSub.Id, out var existingSub))
                {
                    if (existingSub.State != googleSub.State || existingSub.AssignedGrade != googleSub.AssignedGrade)
                    {
                        existingSub.State = googleSub.State;
                        existingSub.AssignedGrade = googleSub.AssignedGrade;
                        allSubmissionsToUpdate.Add(existingSub);
                    }
                }
                else
                {
                    var newSub = mapper.Map<StudentSubmission>(googleSub);
                    newSub.UserId = userId;
                    newSub.AssignmentId = assignmentId;
                    
                    allSubmissionsToAdd.Add(newSub);
                }
            }
        }
        if (allSubmissionsToAdd.Any()) await submissionRepository.AddRangeAsync(allSubmissionsToAdd, cancellationToken);
        if (allSubmissionsToUpdate.Any()) await submissionRepository.UpdateRangeAsync(allSubmissionsToUpdate, cancellationToken);
        
        if (allSubmissionsToAdd.Any() || allSubmissionsToUpdate.Any())
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
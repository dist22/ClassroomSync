using AutoMapper;
using ClassroomSync.Application.Interfaces.Client;
using ClassroomSync.Application.Interfaces.Repositories;
using ClassroomSync.Application.Interfaces.Sevices;
using ClassroomSync.Domain.Entities;

namespace ClassroomSync.Application.Services;

public class AuthService(
    IGoogleAuthClient googleAuthClient,
    IMapper mapper,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    ICourseServiсe courseServiсe,
    IAssignmentService assignmentService,
    ISubmissionService submissionService) : IAuthService
{
    public string GetLoginUrl(string redirectUri)
        => googleAuthClient.GetAuthorizationUrl(redirectUri);

    public async Task<Guid> ProcessGoogleCallbackAsync(string code, string redirectUri,
        CancellationToken cancellationToken = default)
    {
        var googleUser = await googleAuthClient.ExchangeCodeForTokenAsync(code, redirectUri, cancellationToken);

        var user = await userRepository.GetByGoogleIdAsync(googleUser.GoogleId, cancellationToken);
        if (user == null)
        {
            user = mapper.Map<User>(googleUser);
            await userRepository.AddAsync(user, cancellationToken);
        }
        else
        {
            mapper.Map(googleUser, user);
            await userRepository.UpdateAsync(user);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var syncedGoogleCoursesIds =
            await courseServiсe.SyncUserCoursesAsync(user.Id, user.RefreshToken, cancellationToken);
        if (syncedGoogleCoursesIds.Any())
        {
            await assignmentService.SyncAssignmentsForCoursesAsync(syncedGoogleCoursesIds, user.RefreshToken,
                cancellationToken);

            await submissionService.SyncSubmissionsAsync(syncedGoogleCoursesIds, user.Id, user.RefreshToken,
                cancellationToken);
        }

        return user.Id;
    }
}
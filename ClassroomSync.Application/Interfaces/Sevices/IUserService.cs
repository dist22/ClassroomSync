using ClassroomSync.Application.DTOs;

namespace ClassroomSync.Application.Interfaces.Sevices;

public interface IUserService
{
    public Task<IEnumerable<UserDto>> GetAllUsersAsync(CancellationToken cancellationToken);
}
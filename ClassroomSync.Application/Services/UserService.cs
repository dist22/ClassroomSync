using AutoMapper;
using ClassroomSync.Application.DTOs;
using ClassroomSync.Application.Interfaces.Repositories;
using ClassroomSync.Application.Interfaces.Sevices;

namespace ClassroomSync.Application.Services;

public class UserService(IUnitOfWork unitOfWork, IUserRepository userRepository, IMapper mapper) : IUserService
{
    public async Task<IEnumerable<UserDto>> GetAllUsersAsync(CancellationToken cancellationToken) 
        => mapper.Map<IEnumerable<UserDto>>( await userRepository.GetAllAsync(cancellationToken));  
}
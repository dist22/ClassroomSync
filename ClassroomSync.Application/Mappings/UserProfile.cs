using AutoMapper;
using ClassroomSync.Application.DTOs;
using ClassroomSync.Domain.Entities;

namespace ClassroomSync.Application.Mappings;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<GoogleUserInfoDto, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.RefreshToken, opt =>
            {
                opt.PreCondition(src => !string.IsNullOrEmpty(src.RefreshToken));
                opt.MapFrom(src => src.RefreshToken);
            });
    }
}
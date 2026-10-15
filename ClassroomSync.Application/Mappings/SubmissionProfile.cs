using AutoMapper;
using ClassroomSync.Application.DTOs;
using ClassroomSync.Domain.Entities;

namespace ClassroomSync.Application.Mappings;

public class SubmissionProfile : Profile
{
    public SubmissionProfile()
    {
        CreateMap<SubmissionDto, StudentSubmission>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.GoogleSubmissionId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.AssignmentId, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.Assignment, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore());
    }
}
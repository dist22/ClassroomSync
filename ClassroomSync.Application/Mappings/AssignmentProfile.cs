using AutoMapper;
using ClassroomSync.Application.DTOs;
using ClassroomSync.Domain.Entities;

namespace ClassroomSync.Application.Mappings;

public class AssignmentProfile : Profile
{
    public AssignmentProfile()
    {
        CreateMap<AssignmentDto, Assignment>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.GoogleCourseWorkId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.CourseId, opt => opt.Ignore())
            .ForMember(dest => dest.Course, opt => opt.Ignore())
            .ForMember(dest => dest.Submissions, opt => opt.Ignore());
    }
}
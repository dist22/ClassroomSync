using AutoMapper;
using ClassroomSync.Application.DTOs;
using ClassroomSync.Domain.Entities;

namespace ClassroomSync.Application.Mappings;

public class CourseProfile : Profile
{
    public CourseProfile()
    {
        CreateMap<CourseDto, Course>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            
            .ForMember(dest => dest.GoogleCourseId, opt => opt.MapFrom(src => src.Id))
            
            .ForMember(dest => dest.UserCourses, opt => opt.Ignore())
            .ForMember(dest => dest.Assignments, opt => opt.Ignore());
    }
}
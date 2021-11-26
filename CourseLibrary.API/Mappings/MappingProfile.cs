using AutoMapper;
using CourseLibrary.API.DTOs;
using CourseLibrary.API.Entities;
using CourseLibrary.API.ExtensionMethods;

namespace CourseLibrary.API.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Author, AuthorDto>()
                .ForMember(m => m.Age,
                    config =>
                        config.MapFrom(o => o.DateOfBirth.GetAge()))
                .ForMember(m => m.Name,
                    config =>
                        config.MapFrom(p => $"{p.LastName} {p.FirstName}"));

            CreateMap<Course, CourseDto>();

        }
    }
}

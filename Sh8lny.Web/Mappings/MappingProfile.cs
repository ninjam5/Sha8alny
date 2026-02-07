using AutoMapper;
using Sh8lny.Domain.Models;
using Sh8lny.Web.DTOs.MasterData;
using Sh8lny.Web.DTOs.Projects;

namespace Sh8lny.Web.Mappings;

/// <summary>
/// AutoMapper profile containing all mapping configurations.
/// Add your entity-to-DTO mappings here.
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Project mappings
        CreateMap<Project, ProjectResponseDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ProjectID))
            .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.CompanyID))
            .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.CompanyName))
            .ForMember(dest => dest.ProjectType, opt => opt.MapFrom(src => src.ProjectType.HasValue ? src.ProjectType.Value.ToString() : null))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        // Master Data mappings
        CreateMap<Skill, SkillDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.SkillID))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.SkillName))
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.SkillCategory.HasValue ? src.SkillCategory.Value.ToString() : null));

        CreateMap<Department, DepartmentDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.DepartmentID))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.DepartmentName))
            .ForMember(dest => dest.UniversityId, opt => opt.MapFrom(src => src.UniversityID));

        CreateMap<University, UniversityDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.UniversityID))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.UniversityName))
            .ForMember(dest => dest.Logo, opt => opt.MapFrom(src => src.UniversityLogo))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.UniversityType.HasValue ? src.UniversityType.Value.ToString() : null));

        // Add more mappings below as needed
    }
}

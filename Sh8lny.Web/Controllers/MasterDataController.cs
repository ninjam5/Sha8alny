using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Sh8lny.Abstraction.Repositories;
using Sh8lny.Web.DTOs.MasterData;

namespace Sh8lny.Web.Controllers;

/// <summary>
/// Controller for retrieving master/lookup data for dropdowns.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class MasterDataController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public MasterDataController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <summary>
    /// Gets all skills for dropdown selection.
    /// </summary>
    /// <returns>List of skills.</returns>
    [HttpGet("skills")]
    public async Task<ActionResult<IEnumerable<SkillDto>>> GetSkills()
    {
        var skills = await _unitOfWork.Skills.GetAllAsync();
        var dtos = _mapper.Map<IEnumerable<SkillDto>>(skills);
        return Ok(dtos);
    }

    /// <summary>
    /// Gets all departments for dropdown selection.
    /// </summary>
    /// <returns>List of departments.</returns>
    [HttpGet("departments")]
    public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetDepartments()
    {
        var departments = await _unitOfWork.Departments.GetAllAsync();
        var dtos = _mapper.Map<IEnumerable<DepartmentDto>>(departments);
        return Ok(dtos);
    }

    /// <summary>
    /// Gets all universities for dropdown selection.
    /// </summary>
    /// <returns>List of universities.</returns>
    [HttpGet("universities")]
    public async Task<ActionResult<IEnumerable<UniversityDto>>> GetUniversities()
    {
        var universities = await _unitOfWork.Universities.GetAllAsync();
        var dtos = _mapper.Map<IEnumerable<UniversityDto>>(universities);
        return Ok(dtos);
    }
}

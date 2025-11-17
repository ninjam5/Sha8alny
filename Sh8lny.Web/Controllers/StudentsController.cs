using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sh8lny.Application.DTOs.Students;
using Sh8lny.Application.Interfaces;

namespace Sh8lny.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;
    private readonly ILogger<StudentsController> _logger;

    public StudentsController(IStudentService studentService, ILogger<StudentsController> logger)
    {
        _studentService = studentService;
        _logger = logger;
    }

    /// <summary>
    /// Get student by ID
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetStudentById(int id, CancellationToken cancellationToken)
    {
        var result = await _studentService.GetStudentByIdAsync(id, cancellationToken);
        
        if (!result.Success)
            return NotFound(result);
        
        return Ok(result);
    }

    /// <summary>
    /// Get paginated list of students with filters
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetStudents([FromQuery] StudentFilterDto filter, CancellationToken cancellationToken)
    {
        var result = await _studentService.GetStudentsAsync(filter, cancellationToken);
        
        if (!result.Success)
            return BadRequest(result);
        
        return Ok(result);
    }

    /// <summary>
    /// Create student profile
    /// </summary>
    [Authorize(Roles = "Student")]
    [HttpPost]
    public async Task<IActionResult> CreateStudent([FromBody] CreateStudentDto dto, CancellationToken cancellationToken)
    {
        var result = await _studentService.CreateStudentAsync(dto, cancellationToken);
        
        if (!result.Success)
            return BadRequest(result);
        
        return CreatedAtAction(nameof(GetStudentById), new { id = result.Data?.StudentID }, result);
    }

    /// <summary>
    /// Update student profile
    /// </summary>
    [Authorize(Roles = "Student")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateStudent(int id, [FromBody] UpdateStudentDto dto, CancellationToken cancellationToken)
    {
        var result = await _studentService.UpdateStudentAsync(id, dto, cancellationToken);
        
        if (!result.Success)
            return BadRequest(result);
        
        return Ok(result);
    }

    /// <summary>
    /// Delete student profile
    /// </summary>
    [Authorize(Roles = "Student,Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStudent(int id, CancellationToken cancellationToken)
    {
        var result = await _studentService.DeleteStudentAsync(id, cancellationToken);
        
        if (!result.Success)
            return BadRequest(result);
        
        return Ok(result);
    }

    /// <summary>
    /// Add skill to student
    /// </summary>
    [Authorize(Roles = "Student")]
    [HttpPost("{studentId}/skills/{skillId}")]
    public async Task<IActionResult> AddSkill(int studentId, int skillId, CancellationToken cancellationToken)
    {
        var result = await _studentService.AddSkillAsync(studentId, skillId, cancellationToken);
        
        if (!result.Success)
            return BadRequest(result);
        
        return Ok(result);
    }

    /// <summary>
    /// Remove skill from student
    /// </summary>
    [Authorize(Roles = "Student")]
    [HttpDelete("{studentId}/skills/{skillId}")]
    public async Task<IActionResult> RemoveSkill(int studentId, int skillId, CancellationToken cancellationToken)
    {
        var result = await _studentService.RemoveSkillAsync(studentId, skillId, cancellationToken);
        
        if (!result.Success)
            return BadRequest(result);
        
        return Ok(result);
    }

    /// <summary>
    /// Get student's skills
    /// </summary>
    [HttpGet("{studentId}/skills")]
    public async Task<IActionResult> GetStudentSkills(int studentId, CancellationToken cancellationToken)
    {
        var result = await _studentService.GetStudentSkillsAsync(studentId, cancellationToken);
        
        if (!result.Success)
            return NotFound(result);
        
        return Ok(result);
    }
}

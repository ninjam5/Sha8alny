using Sh8lny.Web.DTOs.Base;

namespace Sh8lny.Web.DTOs.MasterData;

/// <summary>
/// DTO for Skill lookup data.
/// </summary>
public class SkillDto : BaseDto
{
    public required string Name { get; set; }
    public string? Category { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}

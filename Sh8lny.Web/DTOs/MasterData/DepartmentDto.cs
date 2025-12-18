using Sh8lny.Web.DTOs.Base;

namespace Sh8lny.Web.DTOs.MasterData;

/// <summary>
/// DTO for Department lookup data.
/// </summary>
public class DepartmentDto : BaseDto
{
    public required string Name { get; set; }
    public int UniversityId { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}

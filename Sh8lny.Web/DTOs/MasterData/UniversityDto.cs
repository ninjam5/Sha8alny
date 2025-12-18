using Sh8lny.Web.DTOs.Base;

namespace Sh8lny.Web.DTOs.MasterData;

/// <summary>
/// DTO for University lookup data.
/// </summary>
public class UniversityDto : BaseDto
{
    public required string Name { get; set; }
    public string? Logo { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? Type { get; set; }
    public bool IsActive { get; set; }
}

namespace Sh8lny.Shared.DTOs.Applications;

/// <summary>
/// DTO for creating a new application.
/// </summary>
public class CreateApplicationDto
{
    public int ProjectId { get; set; }
    public string? Proposal { get; set; }
    public string? Duration { get; set; }
    public decimal BidAmount { get; set; }
}

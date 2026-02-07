namespace Sh8lny.Shared.DTOs.CompanyProfile;

/// <summary>
/// DTO for company profile response.
/// </summary>
public class CompanyDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    
    // Basic info
    public required string CompanyName { get; set; }
    public string? Description { get; set; }
    public string? Industry { get; set; }
    public string? LogoUrl { get; set; }
    
    // Contact info
    public required string ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public string? WebsiteUrl { get; set; }
    
    // Location
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    
    // Metrics
    public decimal AverageRating { get; set; }
    public int TotalReviews { get; set; }
    
    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

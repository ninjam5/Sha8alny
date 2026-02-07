namespace Sh8lny.Shared.DTOs.CompanyProfile;

/// <summary>
/// DTO for creating or updating a company profile.
/// </summary>
public class CreateCompanyProfileDto
{
    public required string CompanyName { get; set; }
    public string? Description { get; set; }
    public string? Industry { get; set; }
    public string? WebsiteUrl { get; set; }
    
    // Location
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    
    // Contact
    public required string ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    
    // Logo (file path string)
    public string? LogoUrl { get; set; }
}

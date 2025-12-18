namespace Sh8lny.Shared.Options;

/// <summary>
/// Configuration options for JWT authentication.
/// </summary>
public class JwtOptions
{
    public const string SectionName = "JwtSettings";

    public required string Key { get; set; }
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
    public int DurationInMinutes { get; set; }
}

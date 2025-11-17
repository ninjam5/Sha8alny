using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Sh8lny.IntegrationTests.Helpers;

/// <summary>
/// Helper class to generate JWT tokens for testing
/// Must match the JWT configuration in appsettings.json
/// </summary>
public static class JwtTokenHelper
{
    private const string SecretKey = "ThisIsAVerySecretKeyForJwtTokenGenerationAndValidation123456";
    private const string Issuer = "Sha8alny";
    private const string Audience = "Sha8alnyUsers";

    /// <summary>
    /// Generates a JWT token for testing
    /// </summary>
    public static string GenerateJwtToken(int userId, string email, string userType, string[]? roles = null)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim("userType", userType),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Ensure the primary user type is also emitted as a role for [Authorize(Roles = ...)] checks
        claims.Add(new Claim(ClaimTypes.Role, userType));

        // Add roles if provided
        if (roles != null)
        {
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Generates a JWT token for a student user
    /// </summary>
    public static string GenerateStudentToken(int userId, string email = "student@test.com")
    {
        return GenerateJwtToken(userId, email, "Student");
    }

    /// <summary>
    /// Generates a JWT token for a company user
    /// </summary>
    public static string GenerateCompanyToken(int userId, string email = "company@test.com")
    {
        return GenerateJwtToken(userId, email, "Company");
    }

    /// <summary>
    /// Generates a JWT token for an admin user
    /// </summary>
    public static string GenerateAdminToken(int userId, string email = "admin@test.com")
    {
        return GenerateJwtToken(userId, email, "Admin", new[] { "Admin" });
    }
}

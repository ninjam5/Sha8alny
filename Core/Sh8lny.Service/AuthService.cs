using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Sh8lny.Abstraction.Repositories;
using Sh8lny.Abstraction.Services;
using Sh8lny.Domain.Models;
using Sh8lny.Shared.DTOs.Auth;
using Sh8lny.Shared.Options;
using BC = BCrypt.Net.BCrypt;

namespace Sh8lny.Service;

/// <summary>
/// Service for handling user authentication (registration and login).
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly JwtOptions _jwtOptions;

    public AuthService(IUnitOfWork unitOfWork, IOptions<JwtOptions> jwtOptions)
    {
        _unitOfWork = unitOfWork;
        _jwtOptions = jwtOptions.Value;
    }

    /// <inheritdoc />
    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        // Check if email already exists
        var existingUser = await _unitOfWork.Users.FindSingleAsync(u => u.Email == dto.Email);
        if (existingUser is not null)
        {
            return new AuthResponseDto
            {
                IsSuccess = false,
                Message = "Email already registered."
            };
        }

        // Parse and validate role
        if (!Enum.TryParse<UserType>(dto.Role, ignoreCase: true, out var userType))
        {
            return new AuthResponseDto
            {
                IsSuccess = false,
                Message = "Invalid role specified."
            };
        }

        // Hash password using BCrypt
        var passwordHash = BC.HashPassword(dto.Password);

        // Create new user
        var user = new User
        {
            Email = dto.Email,
            PasswordHash = passwordHash,
            UserType = userType,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveAsync();

        // Generate JWT token
        var (token, expiration) = GenerateJwtToken(user);

        return new AuthResponseDto
        {
            IsSuccess = true,
            Token = token,
            Expiration = expiration,
            UserId = user.UserID,
            Email = user.Email,
            Role = user.UserType.ToString(),
            Message = "Registration successful."
        };
    }

    /// <inheritdoc />
    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        // Find user by email
        var user = await _unitOfWork.Users.FindSingleAsync(u => u.Email == dto.Email);
        if (user is null)
        {
            return new AuthResponseDto
            {
                IsSuccess = false,
                Message = "Invalid email or password."
            };
        }

        // Verify password using BCrypt
        if (!BC.Verify(dto.Password, user.PasswordHash))
        {
            return new AuthResponseDto
            {
                IsSuccess = false,
                Message = "Invalid email or password."
            };
        }

        // Update last login timestamp
        user.LastLoginAt = DateTime.UtcNow;
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveAsync();

        // Generate JWT token
        var (token, expiration) = GenerateJwtToken(user);

        return new AuthResponseDto
        {
            IsSuccess = true,
            Token = token,
            Expiration = expiration,
            UserId = user.UserID,
            Email = user.Email,
            Role = user.UserType.ToString(),
            Message = "Login successful."
        };
    }

    /// <summary>
    /// Generates a JWT token for the specified user.
    /// </summary>
    /// <param name="user">The user to generate token for.</param>
    /// <returns>Tuple containing token string and expiration date.</returns>
    private (string Token, DateTime Expiration) GenerateJwtToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserID.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.UserType.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        var expiration = DateTime.UtcNow.AddMinutes(_jwtOptions.DurationInMinutes);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: expiration,
            signingCredentials: credentials
        );

        return (new JwtSecurityTokenHandler().WriteToken(token), expiration);
    }
}

using Sh8lny.Application.Common;
using Sh8lny.Application.DTOs.Auth;

namespace Sh8lny.Application.Interfaces;

/// <summary>
/// Authentication and authorization service interface
/// </summary>
public interface IAuthService
{
    Task<ApiResponse<AuthResponseDto>> RegisterStudentAsync(RegisterStudentDto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse<AuthResponseDto>> RegisterCompanyAsync(RegisterCompanyDto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse<AuthResponseDto>> RefreshTokenAsync(RefreshTokenDto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse<bool>> VerifyEmailAsync(string email, string code, CancellationToken cancellationToken = default);
    Task<ApiResponse<bool>> ForgotPasswordAsync(ForgotPasswordDto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse<bool>> ResetPasswordAsync(ResetPasswordDto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse<bool>> ChangePasswordAsync(int userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default);
}

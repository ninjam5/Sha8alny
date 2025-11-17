using Sh8lny.Application.Common;
using Sh8lny.Application.DTOs.Auth;
using Sh8lny.Application.Interfaces;
using Sh8lny.Domain.Entities;
using Sh8lny.Domain.Interfaces;

namespace Sh8lny.Application.UseCases.Auth;

/// <summary>
/// Authentication service implementation
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;

    public AuthService(
        IUnitOfWork unitOfWork,
        ITokenService tokenService,
        IEmailService emailService)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _emailService = emailService;
    }

    public async Task<ApiResponse<AuthResponseDto>> RegisterStudentAsync(RegisterStudentDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check if email already exists
            if (await _unitOfWork.Users.EmailExistsAsync(dto.Email, cancellationToken))
            {
                return ApiResponse<AuthResponseDto>.FailureResponse("Email already registered");
            }

            // Parse academic year
            AcademicYear? academicYear = null;
            if (!string.IsNullOrEmpty(dto.AcademicYear) && Enum.TryParse<AcademicYear>(dto.AcademicYear, out var parsedYear))
            {
                academicYear = parsedYear;
            }

            // Create user
            var user = new User
            {
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                UserType = UserType.Student,
                IsEmailVerified = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Users.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Create student profile
            var student = new Student
            {
                UserID = user.UserID,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Phone = dto.Phone,
                UniversityID = dto.UniversityID,
                DepartmentID = dto.DepartmentID,
                AcademicYear = academicYear,
                City = dto.City,
                Country = dto.Country,
                Status = StudentStatus.Active,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Students.AddAsync(student, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Generate tokens
            var accessToken = _tokenService.GenerateAccessToken(user.UserID, user.Email, user.UserType.ToString());
            var refreshToken = _tokenService.GenerateRefreshToken();

            // Generate verification code
            var verificationCode = Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper();
            user.VerificationCode = verificationCode;
            user.VerificationCodeExpiry = DateTime.UtcNow.AddHours(24);
            await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Send verification email
            await _emailService.SendVerificationEmailAsync(user.Email, verificationCode, cancellationToken);

            var response = new AuthResponseDto
            {
                Token = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                User = new UserInfoDto
                {
                    UserID = user.UserID,
                    Email = user.Email,
                    UserType = user.UserType.ToString(),
                    IsEmailVerified = user.IsEmailVerified,
                    ProfileID = student.StudentID,
                    ProfileName = student.FullName
                }
            };

            return ApiResponse<AuthResponseDto>.SuccessResponse(response, "Student registered successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<AuthResponseDto>.FailureResponse($"Registration failed: {ex.Message}");
        }
    }

    public async Task<ApiResponse<AuthResponseDto>> RegisterCompanyAsync(RegisterCompanyDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check if email already exists
            if (await _unitOfWork.Users.EmailExistsAsync(dto.Email, cancellationToken))
            {
                return ApiResponse<AuthResponseDto>.FailureResponse("Email already registered");
            }

            // Create user
            var user = new User
            {
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                UserType = UserType.Company,
                IsEmailVerified = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Users.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Create company profile
            var company = new Company
            {
                UserID = user.UserID,
                CompanyName = dto.CompanyName,
                ContactEmail = dto.ContactEmail,
                ContactPhone = dto.ContactPhone,
                Industry = dto.Industry,
                Description = dto.Description,
                Website = dto.Website,
                Address = dto.Address,
                City = dto.City,
                Country = dto.Country,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Companies.AddAsync(company, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Generate tokens
            var accessToken = _tokenService.GenerateAccessToken(user.UserID, user.Email, user.UserType.ToString());
            var refreshToken = _tokenService.GenerateRefreshToken();

            // Generate verification code
            var verificationCode = Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper();
            user.VerificationCode = verificationCode;
            user.VerificationCodeExpiry = DateTime.UtcNow.AddHours(24);
            await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Send verification email
            await _emailService.SendVerificationEmailAsync(user.Email, verificationCode, cancellationToken);

            var response = new AuthResponseDto
            {
                Token = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                User = new UserInfoDto
                {
                    UserID = user.UserID,
                    Email = user.Email,
                    UserType = user.UserType.ToString(),
                    IsEmailVerified = user.IsEmailVerified,
                    ProfileID = company.CompanyID,
                    ProfileName = company.CompanyName
                }
            };

            return ApiResponse<AuthResponseDto>.SuccessResponse(response, "Company registered successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<AuthResponseDto>.FailureResponse($"Registration failed: {ex.Message}");
        }
    }

    public async Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(dto.Email, cancellationToken);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                return ApiResponse<AuthResponseDto>.FailureResponse("Invalid email or password");
            }

            // Update last login
            user.LastLoginAt = DateTime.UtcNow;
            await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Generate tokens
            var accessToken = _tokenService.GenerateAccessToken(user.UserID, user.Email, user.UserType.ToString());
            var refreshToken = _tokenService.GenerateRefreshToken();

            // Get profile info
            string? profileName = null;
            int? profileID = null;

            if (user.UserType == UserType.Student)
            {
                var student = await _unitOfWork.Students.GetByUserIdAsync(user.UserID, cancellationToken);
                if (student != null)
                {
                    profileName = student.FullName;
                    profileID = student.StudentID;
                }
            }
            else if (user.UserType == UserType.Company)
            {
                var company = await _unitOfWork.Companies.GetByUserIdAsync(user.UserID, cancellationToken);
                if (company != null)
                {
                    profileName = company.CompanyName;
                    profileID = company.CompanyID;
                }
            }

            var response = new AuthResponseDto
            {
                Token = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                User = new UserInfoDto
                {
                    UserID = user.UserID,
                    Email = user.Email,
                    UserType = user.UserType.ToString(),
                    IsEmailVerified = user.IsEmailVerified,
                    ProfileID = profileID,
                    ProfileName = profileName
                }
            };

            return ApiResponse<AuthResponseDto>.SuccessResponse(response, "Login successful");
        }
        catch (Exception ex)
        {
            return ApiResponse<AuthResponseDto>.FailureResponse($"Login failed: {ex.Message}");
        }
    }

    public async Task<ApiResponse<AuthResponseDto>> RefreshTokenAsync(RefreshTokenDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!_tokenService.ValidateToken(dto.RefreshToken))
            {
                return ApiResponse<AuthResponseDto>.FailureResponse("Invalid refresh token");
            }

            var userId = _tokenService.GetUserIdFromToken(dto.RefreshToken);
            if (userId == null)
            {
                return ApiResponse<AuthResponseDto>.FailureResponse("Invalid refresh token");
            }

            var user = await _unitOfWork.Users.GetByIdAsync(userId.Value, cancellationToken);
            if (user == null)
            {
                return ApiResponse<AuthResponseDto>.FailureResponse("User not found");
            }

            var accessToken = _tokenService.GenerateAccessToken(user.UserID, user.Email, user.UserType.ToString());
            var refreshToken = _tokenService.GenerateRefreshToken();

            var response = new AuthResponseDto
            {
                Token = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                User = new UserInfoDto
                {
                    UserID = user.UserID,
                    Email = user.Email,
                    UserType = user.UserType.ToString(),
                    IsEmailVerified = user.IsEmailVerified
                }
            };

            return ApiResponse<AuthResponseDto>.SuccessResponse(response, "Token refreshed successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<AuthResponseDto>.FailureResponse($"Token refresh failed: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> VerifyEmailAsync(string email, string code, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(email, cancellationToken);
            if (user == null)
            {
                return ApiResponse<bool>.FailureResponse("User not found");
            }

            if (user.IsEmailVerified)
            {
                return ApiResponse<bool>.SuccessResponse(true, "Email already verified");
            }

            if (user.VerificationCode != code)
            {
                return ApiResponse<bool>.FailureResponse("Invalid verification code");
            }

            if (user.VerificationCodeExpiry < DateTime.UtcNow)
            {
                return ApiResponse<bool>.FailureResponse("Verification code expired");
            }

            user.IsEmailVerified = true;
            user.VerificationCode = null;
            user.VerificationCodeExpiry = null;
            user.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.SuccessResponse(true, "Email verified successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.FailureResponse($"Email verification failed: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> ForgotPasswordAsync(ForgotPasswordDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(dto.Email, cancellationToken);
            if (user == null)
            {
                return ApiResponse<bool>.SuccessResponse(true, "If the email exists, a password reset code has been sent");
            }

            var resetCode = Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper();
            user.VerificationCode = resetCode;
            user.VerificationCodeExpiry = DateTime.UtcNow.AddHours(1);
            await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _emailService.SendPasswordResetEmailAsync(user.Email, resetCode, cancellationToken);

            return ApiResponse<bool>.SuccessResponse(true, "Password reset code sent");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.FailureResponse($"Password reset request failed: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> ResetPasswordAsync(ResetPasswordDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(dto.Email, cancellationToken);
            if (user == null)
            {
                return ApiResponse<bool>.FailureResponse("User not found");
            }

            if (user.VerificationCode != dto.Code)
            {
                return ApiResponse<bool>.FailureResponse("Invalid reset code");
            }

            if (user.VerificationCodeExpiry < DateTime.UtcNow)
            {
                return ApiResponse<bool>.FailureResponse("Reset code expired");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            user.VerificationCode = null;
            user.VerificationCodeExpiry = null;
            user.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.SuccessResponse(true, "Password reset successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.FailureResponse($"Password reset failed: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> ChangePasswordAsync(int userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                return ApiResponse<bool>.FailureResponse("User not found");
            }

            if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
            {
                return ApiResponse<bool>.FailureResponse("Current password is incorrect");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.SuccessResponse(true, "Password changed successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.FailureResponse($"Password change failed: {ex.Message}");
        }
    }
}

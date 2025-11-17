using System.ComponentModel.DataAnnotations;

namespace Sh8lny.Application.DTOs.Auth;

public class RegisterStudentDto
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [MaxLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#])[A-Za-z\d@$!%*?&#]{8,}$", 
        ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character")]
    public required string Password { get; set; }

    [Required(ErrorMessage = "Confirm Password is required")]
    [Compare("Password", ErrorMessage = "Password and Confirm Password do not match")]
    public required string ConfirmPassword { get; set; }

    [Required(ErrorMessage = "First Name is required")]
    [MinLength(2, ErrorMessage = "First Name must be at least 2 characters")]
    [MaxLength(50, ErrorMessage = "First Name cannot exceed 50 characters")]
    public required string FirstName { get; set; }

    [Required(ErrorMessage = "Last Name is required")]
    [MinLength(2, ErrorMessage = "Last Name must be at least 2 characters")]
    [MaxLength(50, ErrorMessage = "Last Name cannot exceed 50 characters")]
    public required string LastName { get; set; }

    [Phone(ErrorMessage = "Invalid phone number format")]
    [MaxLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
    public string? Phone { get; set; }

    public int? UniversityID { get; set; }
    public int? DepartmentID { get; set; }

    [MaxLength(50, ErrorMessage = "Academic Year cannot exceed 50 characters")]
    public string? AcademicYear { get; set; }

    [MaxLength(100, ErrorMessage = "City cannot exceed 100 characters")]
    public string? City { get; set; }

    [Required(ErrorMessage = "Country is required")]
    [MaxLength(100, ErrorMessage = "Country cannot exceed 100 characters")]
    public required string Country { get; set; }
}

public class RegisterCompanyDto
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [MaxLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#])[A-Za-z\d@$!%*?&#]{8,}$", 
        ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character")]
    public required string Password { get; set; }

    [Required(ErrorMessage = "Confirm Password is required")]
    [Compare("Password", ErrorMessage = "Password and Confirm Password do not match")]
    public required string ConfirmPassword { get; set; }

    [Required(ErrorMessage = "Company Name is required")]
    [MinLength(2, ErrorMessage = "Company Name must be at least 2 characters")]
    [MaxLength(200, ErrorMessage = "Company Name cannot exceed 200 characters")]
    public required string CompanyName { get; set; }

    [Required(ErrorMessage = "Contact Email is required")]
    [EmailAddress(ErrorMessage = "Invalid contact email format")]
    [MaxLength(100, ErrorMessage = "Contact Email cannot exceed 100 characters")]
    public required string ContactEmail { get; set; }

    [Phone(ErrorMessage = "Invalid phone number format")]
    [MaxLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
    public string? ContactPhone { get; set; }

    [Url(ErrorMessage = "Invalid website URL format")]
    [MaxLength(500, ErrorMessage = "Website URL cannot exceed 500 characters")]
    public string? Website { get; set; }

    [MaxLength(100, ErrorMessage = "Industry cannot exceed 100 characters")]
    public string? Industry { get; set; }

    [MaxLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
    public string? Description { get; set; }

    [MaxLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
    public string? Address { get; set; }

    [MaxLength(100, ErrorMessage = "City cannot exceed 100 characters")]
    public string? City { get; set; }

    [MaxLength(100, ErrorMessage = "Country cannot exceed 100 characters")]
    public string? Country { get; set; }
}

public class LoginDto
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public required string Password { get; set; }

    public bool RememberMe { get; set; }
}

public class AuthResponseDto
{
    public required string Token { get; set; }
    public required string RefreshToken { get; set; }
    public DateTime ExpiresAt { get; set; }
    public required UserInfoDto User { get; set; }
}

public class UserInfoDto
{
    public int UserID { get; set; }
    public required string Email { get; set; }
    public required string UserType { get; set; }
    public bool IsEmailVerified { get; set; }
    public int? ProfileID { get; set; }
    public string? ProfileName { get; set; }
}

public class RefreshTokenDto
{
    [Required(ErrorMessage = "Token is required")]
    public required string Token { get; set; }

    [Required(ErrorMessage = "Refresh Token is required")]
    public required string RefreshToken { get; set; }
}

public class ForgotPasswordDto
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public required string Email { get; set; }
}

public class ResetPasswordDto
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "Verification code is required")]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "Code must be exactly 6 characters")]
    public required string Code { get; set; }

    [Required(ErrorMessage = "New Password is required")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#])[A-Za-z\d@$!%*?&#]{8,}$", 
        ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character")]
    public required string NewPassword { get; set; }

    [Required(ErrorMessage = "Confirm Password is required")]
    [Compare("NewPassword", ErrorMessage = "Password and Confirm Password do not match")]
    public required string ConfirmPassword { get; set; }
}

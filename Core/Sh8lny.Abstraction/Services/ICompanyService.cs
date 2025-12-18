using Sh8lny.Shared.DTOs.Common;
using Sh8lny.Shared.DTOs.CompanyProfile;

namespace Sh8lny.Abstraction.Services;

/// <summary>
/// Interface for company profile operations.
/// </summary>
public interface ICompanyService
{
    /// <summary>
    /// Creates a new company profile or updates an existing one.
    /// </summary>
    /// <param name="userId">The ID of the user creating/updating the profile.</param>
    /// <param name="dto">The company profile data.</param>
    /// <returns>Service response containing the company ID.</returns>
    Task<ServiceResponse<int>> CreateOrUpdateProfileAsync(int userId, CreateCompanyProfileDto dto);

    /// <summary>
    /// Gets the company profile for a user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>Service response containing the company profile.</returns>
    Task<ServiceResponse<CompanyDto>> GetProfileAsync(int userId);
}

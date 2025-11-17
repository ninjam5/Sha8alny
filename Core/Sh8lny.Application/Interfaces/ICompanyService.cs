using Sh8lny.Application.Common;
using Sh8lny.Application.DTOs.Companies;

namespace Sh8lny.Application.Interfaces;

/// <summary>
/// Company management service interface
/// </summary>
public interface ICompanyService
{
    /// <summary>
    /// Get company profile by ID
    /// </summary>
    Task<ApiResponse<CompanyProfileDto>> GetCompanyByIdAsync(int companyId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get company profile by user ID
    /// </summary>
    Task<ApiResponse<CompanyProfileDto>> GetCompanyByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get paginated list of companies with filters
    /// </summary>
    Task<ApiResponse<PagedResult<CompanyListDto>>> GetCompaniesAsync(CompanyFilterDto filter, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Create new company profile
    /// </summary>
    Task<ApiResponse<CompanyProfileDto>> CreateCompanyAsync(CreateCompanyDto dto, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Update company profile
    /// </summary>
    Task<ApiResponse<CompanyProfileDto>> UpdateCompanyAsync(int companyId, UpdateCompanyDto dto, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Delete company profile
    /// </summary>
    Task<ApiResponse<bool>> DeleteCompanyAsync(int companyId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Verify company (admin only)
    /// </summary>
    Task<ApiResponse<bool>> VerifyCompanyAsync(int companyId, CancellationToken cancellationToken = default);
}

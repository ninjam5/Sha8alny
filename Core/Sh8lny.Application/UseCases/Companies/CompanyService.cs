using Sh8lny.Application.Common;
using Sh8lny.Application.DTOs.Companies;
using Sh8lny.Application.Interfaces;
using Sh8lny.Domain.Entities;
using Sh8lny.Domain.Interfaces;

namespace Sh8lny.Application.UseCases.Companies;

public class CompanyService : ICompanyService
{
    private readonly IUnitOfWork _unitOfWork;

    public CompanyService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<CompanyProfileDto>> GetCompanyByIdAsync(int companyId, CancellationToken cancellationToken = default)
    {
        try
        {
            var company = await _unitOfWork.Companies.GetByIdAsync(companyId, cancellationToken);
            if (company == null)
                return ApiResponse<CompanyProfileDto>.FailureResponse("Company not found");

            var user = await _unitOfWork.Users.GetByIdAsync(company.UserID, cancellationToken);
            var projects = await _unitOfWork.Projects.GetByCompanyIdAsync(companyId, cancellationToken);

            var dto = new CompanyProfileDto
            {
                CompanyID = company.CompanyID,
                UserID = company.UserID,
                CompanyName = company.CompanyName,
                Email = user?.Email ?? company.ContactEmail,
                PhoneNumber = company.ContactPhone,
                LogoURL = company.CompanyLogo,
                Description = company.Description,
                Industry = company.Industry,
                CompanySize = null, // Not in entity
                FoundedYear = null, // Not in entity
                Website = company.Website,
                Address = company.Address,
                City = company.City,
                Country = company.Country,
                LinkedInProfile = null, // Not in entity
                TwitterProfile = null, // Not in entity
                FacebookProfile = null, // Not in entity
                TotalProjects = projects.Count(),
                ActiveProjects = projects.Count(p => p.Status == ProjectStatus.Active),
                TotalHires = 0, // Will need Applications with Accepted status
                TotalReviews = company.TotalReviews,
                AverageRating = company.AverageRating,
                IsVerified = false, // Removed from entity
                CreatedAt = company.CreatedAt,
                LastLoginAt = user?.LastLoginAt
            };

            return ApiResponse<CompanyProfileDto>.SuccessResponse(dto, "Company retrieved successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<CompanyProfileDto>.FailureResponse($"Error retrieving company: {ex.Message}");
        }
    }

    public async Task<ApiResponse<CompanyProfileDto>> GetCompanyByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var company = await _unitOfWork.Companies.GetByUserIdAsync(userId, cancellationToken);
            if (company == null)
                return ApiResponse<CompanyProfileDto>.FailureResponse("Company not found");

            return await GetCompanyByIdAsync(company.CompanyID, cancellationToken);
        }
        catch (Exception ex)
        {
            return ApiResponse<CompanyProfileDto>.FailureResponse($"Error retrieving company: {ex.Message}");
        }
    }

    public async Task<ApiResponse<PagedResult<CompanyListDto>>> GetCompaniesAsync(
        CompanyFilterDto filter, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var companies = await _unitOfWork.Companies.GetAllAsync(cancellationToken);

            // Apply filters
            if (!string.IsNullOrWhiteSpace(filter.Industry))
                companies = companies.Where(c => c.Industry != null && c.Industry.ToLower() == filter.Industry.ToLower());

            if (!string.IsNullOrWhiteSpace(filter.City))
                companies = companies.Where(c => c.City != null && c.City.ToLower() == filter.City.ToLower());

            if (filter.MinRating.HasValue)
                companies = companies.Where(c => c.AverageRating >= filter.MinRating.Value);

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var searchLower = filter.SearchTerm.ToLower();
                companies = companies.Where(c =>
                    c.CompanyName.ToLower().Contains(searchLower) ||
                    (c.Description != null && c.Description.ToLower().Contains(searchLower)) ||
                    (c.Industry != null && c.Industry.ToLower().Contains(searchLower)));
            }

            var totalCount = companies.Count();

            // Pagination
            var paginatedCompanies = companies
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

            var companyDtos = paginatedCompanies.Select(c => new CompanyListDto
            {
                CompanyID = c.CompanyID,
                CompanyName = c.CompanyName,
                LogoURL = c.CompanyLogo,
                Industry = c.Industry,
                City = c.City,
                ActiveProjects = 0,
                AverageRating = c.AverageRating,
                TotalReviews = c.TotalReviews,
                IsVerified = false // Removed from entity
            }).ToList();

            var result = new PagedResult<CompanyListDto>
            {
                Items = companyDtos,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };

            return ApiResponse<PagedResult<CompanyListDto>>.SuccessResponse(result, "Companies retrieved successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<PagedResult<CompanyListDto>>.FailureResponse($"Error retrieving companies: {ex.Message}");
        }
    }

    public async Task<ApiResponse<CompanyProfileDto>> CreateCompanyAsync(CreateCompanyDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate user exists
            var user = await _unitOfWork.Users.GetByIdAsync(dto.UserID, cancellationToken);
            if (user == null)
                return ApiResponse<CompanyProfileDto>.FailureResponse("User not found");

            // Check if company already exists for this user
            var existingCompany = await _unitOfWork.Companies.GetByUserIdAsync(dto.UserID, cancellationToken);
            if (existingCompany != null)
                return ApiResponse<CompanyProfileDto>.FailureResponse("Company profile already exists for this user");

            var company = new Company
            {
                UserID = dto.UserID,
                CompanyName = user.Email.Split('@')[0], // Default from email
                ContactEmail = user.Email,
                ContactPhone = dto.PhoneNumber,
                Website = dto.Website,
                Industry = dto.Industry,
                Description = dto.Description,
                Address = dto.Address,
                City = dto.City,
                Country = dto.Country ?? "Unknown",
                AverageRating = 0,
                TotalReviews = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Companies.AddAsync(company, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return await GetCompanyByIdAsync(company.CompanyID, cancellationToken);
        }
        catch (Exception ex)
        {
            return ApiResponse<CompanyProfileDto>.FailureResponse($"Error creating company: {ex.Message}");
        }
    }

    public async Task<ApiResponse<CompanyProfileDto>> UpdateCompanyAsync(int companyId, UpdateCompanyDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var company = await _unitOfWork.Companies.GetByIdAsync(companyId, cancellationToken);
            if (company == null)
                return ApiResponse<CompanyProfileDto>.FailureResponse("Company not found");

            company.ContactPhone = dto.PhoneNumber;
            company.CompanyLogo = dto.LogoURL;
            company.Website = dto.Website;
            company.Industry = dto.Industry;
            company.Description = dto.Description;
            company.Address = dto.Address;
            company.City = dto.City;
            company.Country = dto.Country;
            company.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Companies.UpdateAsync(company, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return await GetCompanyByIdAsync(companyId, cancellationToken);
        }
        catch (Exception ex)
        {
            return ApiResponse<CompanyProfileDto>.FailureResponse($"Error updating company: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> DeleteCompanyAsync(int companyId, CancellationToken cancellationToken = default)
    {
        try
        {
            var company = await _unitOfWork.Companies.GetByIdAsync(companyId, cancellationToken);
            if (company == null)
                return ApiResponse<bool>.FailureResponse("Company not found");

            await _unitOfWork.Companies.DeleteAsync(companyId, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.SuccessResponse(true, "Company deleted successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.FailureResponse($"Error deleting company: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> VerifyCompanyAsync(int companyId, CancellationToken cancellationToken = default)
    {
        try
        {
            var company = await _unitOfWork.Companies.GetByIdAsync(companyId, cancellationToken);
            if (company == null)
                return ApiResponse<bool>.FailureResponse("Company not found");

            // Note: IsVerified property was removed from Company entity
            // This method would need to be updated when verification is reimplemented
            company.UpdatedAt = DateTime.UtcNow;
            
            await _unitOfWork.Companies.UpdateAsync(company, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.SuccessResponse(true, "Company verification status updated");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.FailureResponse($"Error verifying company: {ex.Message}");
        }
    }
}

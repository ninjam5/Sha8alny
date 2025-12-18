using Sh8lny.Abstraction.Repositories;
using Sh8lny.Abstraction.Services;
using Sh8lny.Domain.Models;
using Sh8lny.Shared.DTOs.Common;
using Sh8lny.Shared.DTOs.CompanyProfile;

namespace Sh8lny.Service;

/// <summary>
/// Service for company profile operations.
/// </summary>
public class CompanyService : ICompanyService
{
    private readonly IUnitOfWork _unitOfWork;

    public CompanyService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<ServiceResponse<int>> CreateOrUpdateProfileAsync(int userId, CreateCompanyProfileDto dto)
    {
        try
        {
            // Check if user exists
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user is null)
            {
                return ServiceResponse<int>.Failure("User not found.");
            }

            // Check if company profile already exists for this user
            var existingCompany = await _unitOfWork.Companies.FindSingleAsync(c => c.UserID == userId);

            if (existingCompany is not null)
            {
                // Update existing company
                existingCompany.CompanyName = dto.CompanyName;
                existingCompany.Description = dto.Description;
                existingCompany.Industry = dto.Industry;
                existingCompany.Website = dto.WebsiteUrl;
                existingCompany.Address = dto.Address;
                existingCompany.City = dto.City;
                existingCompany.State = dto.State;
                existingCompany.Country = dto.Country;
                existingCompany.ContactEmail = dto.ContactEmail;
                existingCompany.ContactPhone = dto.ContactPhone;
                existingCompany.CompanyLogo = dto.LogoUrl;
                existingCompany.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Companies.Update(existingCompany);
                await _unitOfWork.SaveAsync();

                return ServiceResponse<int>.Success(existingCompany.CompanyID, "Company profile updated successfully.");
            }
            else
            {
                // Create new company
                var company = new Company
                {
                    UserID = userId,
                    CompanyName = dto.CompanyName,
                    Description = dto.Description,
                    Industry = dto.Industry,
                    Website = dto.WebsiteUrl,
                    Address = dto.Address,
                    City = dto.City,
                    State = dto.State,
                    Country = dto.Country,
                    ContactEmail = dto.ContactEmail,
                    ContactPhone = dto.ContactPhone,
                    CompanyLogo = dto.LogoUrl,
                    AverageRating = 0,
                    TotalReviews = 0,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Companies.AddAsync(company);
                await _unitOfWork.SaveAsync();

                return ServiceResponse<int>.Success(company.CompanyID, "Company profile created successfully.");
            }
        }
        catch (Exception ex)
        {
            return ServiceResponse<int>.Failure("An error occurred while saving the company profile.",
                new List<string> { ex.Message });
        }
    }

    /// <inheritdoc />
    public async Task<ServiceResponse<CompanyDto>> GetProfileAsync(int userId)
    {
        try
        {
            var company = await _unitOfWork.Companies.FindSingleAsync(c => c.UserID == userId);

            if (company is null)
            {
                return ServiceResponse<CompanyDto>.Failure("Company profile not found.");
            }

            var companyDto = new CompanyDto
            {
                Id = company.CompanyID,
                UserId = company.UserID,
                CompanyName = company.CompanyName,
                Description = company.Description,
                Industry = company.Industry,
                LogoUrl = company.CompanyLogo,
                ContactEmail = company.ContactEmail,
                ContactPhone = company.ContactPhone,
                WebsiteUrl = company.Website,
                Address = company.Address,
                City = company.City,
                State = company.State,
                Country = company.Country,
                AverageRating = company.AverageRating,
                TotalReviews = company.TotalReviews,
                CreatedAt = company.CreatedAt,
                UpdatedAt = company.UpdatedAt
            };

            return ServiceResponse<CompanyDto>.Success(companyDto);
        }
        catch (Exception ex)
        {
            return ServiceResponse<CompanyDto>.Failure("An error occurred while retrieving the company profile.",
                new List<string> { ex.Message });
        }
    }
}

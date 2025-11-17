using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sh8lny.Application.DTOs.Companies;
using Sh8lny.Application.Interfaces;

namespace Sh8lny.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CompaniesController : ControllerBase
{
    private readonly ICompanyService _companyService;
    private readonly ILogger<CompaniesController> _logger;

    public CompaniesController(ICompanyService companyService, ILogger<CompaniesController> logger)
    {
        _companyService = companyService;
        _logger = logger;
    }

    /// <summary>
    /// Get company by ID
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCompanyById(int id, CancellationToken cancellationToken)
    {
        var result = await _companyService.GetCompanyByIdAsync(id, cancellationToken);
        
        if (!result.Success)
            return NotFound(result);
        
        return Ok(result);
    }

    /// <summary>
    /// Get paginated list of companies with filters
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetCompanies([FromQuery] CompanyFilterDto filter, CancellationToken cancellationToken)
    {
        var result = await _companyService.GetCompaniesAsync(filter, cancellationToken);
        
        if (!result.Success)
            return BadRequest(result);
        
        return Ok(result);
    }

    /// <summary>
    /// Create company profile
    /// </summary>
    [Authorize(Roles = "Company")]
    [HttpPost]
    public async Task<IActionResult> CreateCompany([FromBody] CreateCompanyDto dto, CancellationToken cancellationToken)
    {
        var result = await _companyService.CreateCompanyAsync(dto, cancellationToken);
        
        if (!result.Success)
            return BadRequest(result);
        
        return CreatedAtAction(nameof(GetCompanyById), new { id = result.Data?.CompanyID }, result);
    }

    /// <summary>
    /// Update company profile
    /// </summary>
    [Authorize(Roles = "Company")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCompany(int id, [FromBody] UpdateCompanyDto dto, CancellationToken cancellationToken)
    {
        var result = await _companyService.UpdateCompanyAsync(id, dto, cancellationToken);
        
        if (!result.Success)
            return BadRequest(result);
        
        return Ok(result);
    }

    /// <summary>
    /// Delete company profile
    /// </summary>
    [Authorize(Roles = "Company,Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCompany(int id, CancellationToken cancellationToken)
    {
        var result = await _companyService.DeleteCompanyAsync(id, cancellationToken);
        
        if (!result.Success)
            return BadRequest(result);
        
        return Ok(result);
    }

    /// <summary>
    /// Verify company (admin only)
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpPost("{id}/verify")]
    public async Task<IActionResult> VerifyCompany(int id, CancellationToken cancellationToken)
    {
        var result = await _companyService.VerifyCompanyAsync(id, cancellationToken);
        
        if (!result.Success)
            return BadRequest(result);
        
        return Ok(result);
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sh8lny.Application.DTOs.Certificates;
using Sh8lny.Application.Interfaces;
using System.Security.Claims;

namespace Sh8lny.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CertificatesController : ControllerBase
{
    private readonly ICertificateService _certificateService;
    private readonly ILogger<CertificatesController> _logger;

    public CertificatesController(ICertificateService certificateService, ILogger<CertificatesController> logger)
    {
        _certificateService = certificateService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> IssueCertificate([FromBody] IssueCertificateDto dto)
    {
        var result = await _certificateService.IssueCertificateAsync(dto);
        return CreatedAtAction(nameof(GetCertificate), new { id = result.CertificateID }, result);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCertificate(int id)
    {
        var result = await _certificateService.GetCertificateByIdAsync(id);
        return Ok(result);
    }

    [HttpGet("by-number/{certificateNumber}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCertificateByNumber(string certificateNumber)
    {
        var result = await _certificateService.GetCertificateByNumberAsync(certificateNumber);
        return Ok(result);
    }

    [HttpGet("student/{studentId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetStudentCertificates(int studentId)
    {
        var result = await _certificateService.GetStudentCertificatesAsync(studentId);
        return Ok(result);
    }

    [HttpGet("company/{companyId}")]
    public async Task<IActionResult> GetCompanyCertificates(int companyId)
    {
        var result = await _certificateService.GetCompanyCertificatesAsync(companyId);
        return Ok(result);
    }

    [HttpGet("project/{projectId}")]
    public async Task<IActionResult> GetProjectCertificates(int projectId)
    {
        var result = await _certificateService.GetProjectCertificatesAsync(projectId);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCertificate(int id, [FromBody] UpdateCertificateDto dto)
    {
        var result = await _certificateService.UpdateCertificateAsync(dto);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCertificate(int id)
    {
        await _certificateService.DeleteCertificateAsync(id);
        return NoContent();
    }

    [HttpPost("verify")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyCertificate([FromBody] VerifyCertificateDto dto)
    {
        var result = await _certificateService.VerifyCertificateAsync(dto);
        return Ok(result);
    }
}

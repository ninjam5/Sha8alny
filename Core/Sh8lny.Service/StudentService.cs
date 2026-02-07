using Sh8lny.Abstraction.Repositories;
using Sh8lny.Abstraction.Services;
using Sh8lny.Domain.Models;
using Sh8lny.Shared.DTOs.Common;
using Sh8lny.Shared.DTOs.StudentProfile;

namespace Sh8lny.Service;

/// <summary>
/// Service for student profile operations.
/// </summary>
public class StudentService : IStudentService
{
    private readonly IUnitOfWork _unitOfWork;

    public StudentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<ServiceResponse<int>> CreateProfileAsync(int userId, CreateStudentProfileDto dto)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            // Check if user exists
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user is null)
            {
                return ServiceResponse<int>.Failure("User not found.");
            }

            // Check if student profile already exists for this user
            var existingStudent = await _unitOfWork.Students.FindSingleAsync(s => s.UserID == userId);
            if (existingStudent is not null)
            {
                return ServiceResponse<int>.Failure("Student profile already exists for this user.");
            }

            // Parse full name
            var nameParts = dto.FullName.Trim().Split(' ', 2);
            var firstName = nameParts[0];
            var lastName = nameParts.Length > 1 ? nameParts[1] : string.Empty;

            // Create Student entity
            var student = new Student
            {
                UserID = userId,
                FirstName = firstName,
                LastName = lastName,
                Phone = dto.Phone,
                ProfilePicture = dto.ProfilePicture,
                GitHubProfile = dto.GitHubProfile,
                City = dto.City,
                State = dto.State,
                Country = dto.Country,
                Status = StudentStatus.Active,
                ProfileCompleteness = CalculateProfileCompleteness(dto),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Students.AddAsync(student);
            await _unitOfWork.SaveAsync();

            // Add Education items
            foreach (var educationDto in dto.Educations)
            {
                var education = new Education
                {
                    StudentID = student.StudentID,
                    UniversityName = educationDto.UniversityName,
                    Degree = educationDto.Degree,
                    FieldOfStudy = educationDto.FieldOfStudy,
                    StartYear = educationDto.StartYear,
                    EndYear = educationDto.EndYear,
                    Description = educationDto.Description,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _unitOfWork.Educations.AddAsync(education);
            }

            // Add Experience items
            foreach (var experienceDto in dto.Experiences)
            {
                var experience = new Experience
                {
                    StudentID = student.StudentID,
                    CompanyName = experienceDto.CompanyName,
                    Role = experienceDto.Role,
                    Location = experienceDto.Location,
                    StartDate = experienceDto.StartDate,
                    EndDate = experienceDto.EndDate,
                    IsCurrent = experienceDto.IsCurrent,
                    Description = experienceDto.Description,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _unitOfWork.Experiences.AddAsync(experience);
            }

            // Add StudentSkill items (linking Student to Skills)
            foreach (var skillId in dto.SkillIds.Distinct())
            {
                // Verify skill exists
                var skill = await _unitOfWork.Skills.GetByIdAsync(skillId);
                if (skill is null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ServiceResponse<int>.Failure($"Skill with ID {skillId} not found.");
                }

                var studentSkill = new StudentSkill
                {
                    StudentID = student.StudentID,
                    SkillID = skillId,
                    ProficiencyLevel = ProficiencyLevel.Beginner,
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.StudentSkills.AddAsync(studentSkill);
            }

            await _unitOfWork.SaveAsync();
            await _unitOfWork.CommitTransactionAsync();

            return ServiceResponse<int>.Success(student.StudentID, "Student profile created successfully.");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            return ServiceResponse<int>.Failure("An error occurred while creating the profile.", 
                new List<string> { ex.Message });
        }
    }

    /// <summary>
    /// Calculates profile completeness percentage.
    /// </summary>
    private static int CalculateProfileCompleteness(CreateStudentProfileDto dto)
    {
        var completeness = 0;
        var totalFields = 8;

        if (!string.IsNullOrWhiteSpace(dto.FullName)) completeness++;
        if (!string.IsNullOrWhiteSpace(dto.Bio)) completeness++;
        if (!string.IsNullOrWhiteSpace(dto.Phone)) completeness++;
        if (!string.IsNullOrWhiteSpace(dto.ProfilePicture)) completeness++;
        if (!string.IsNullOrWhiteSpace(dto.Country)) completeness++;
        if (dto.Educations.Count > 0) completeness++;
        if (dto.Experiences.Count > 0) completeness++;
        if (dto.SkillIds.Count > 0) completeness++;

        return (int)((completeness / (double)totalFields) * 100);
    }
}

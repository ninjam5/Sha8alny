using Sh8lny.Application.Common;
using Sh8lny.Application.DTOs.Students;
using Sh8lny.Application.Interfaces;
using Sh8lny.Domain.Entities;
using Sh8lny.Domain.Interfaces;

namespace Sh8lny.Application.UseCases.Students;

public class StudentService : IStudentService
{
    private readonly IUnitOfWork _unitOfWork;

    public StudentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<StudentProfileDto>> GetStudentByIdAsync(int studentId, CancellationToken cancellationToken = default)
    {
        try
        {
            var student = await _unitOfWork.Students.GetByIdAsync(studentId, cancellationToken);
            if (student == null)
                return ApiResponse<StudentProfileDto>.FailureResponse("Student not found");

            var user = await _unitOfWork.Users.GetByIdAsync(student.UserID, cancellationToken);
            var skills = await _unitOfWork.StudentSkills.GetByStudentIdAsync(student.StudentID, cancellationToken);
            var applications = await _unitOfWork.Applications.GetByStudentIdAsync(student.StudentID, cancellationToken);

            var dto = new StudentProfileDto
            {
                StudentID = student.StudentID,
                UserID = student.UserID,
                FullName = student.FullName,
                Email = user?.Email ?? string.Empty,
                PhoneNumber = student.Phone,
                ProfilePicture = student.ProfilePicture,
                UniversityID = student.UniversityID,
                UniversityName = student.University?.UniversityName,
                DepartmentID = student.DepartmentID,
                DepartmentName = student.Department?.DepartmentName,
                AcademicYear = student.AcademicYear?.ToString(),
                ResumeURL = null, // Not in Student entity
                PortfolioURL = null, // Not in Student entity
                LinkedInProfile = null, // Not in Student entity
                GitHubProfile = null, // Not in Student entity
                TotalApplications = applications.Count(),
                AcceptedApplications = applications.Count(a => a.Status == ApplicationStatus.Accepted),
                CompletedProjects = 0, // Will need CompletedOpportunities count
                TotalReviews = student.TotalReviews,
                AverageRating = student.AverageRating,
                Skills = skills.Select(ss => new SkillDto
                {
                    SkillID = ss.Skill.SkillID,
                    SkillName = ss.Skill.SkillName,
                    Category = ss.Skill.SkillCategory?.ToString()
                }).ToList(),
                CreatedAt = student.CreatedAt,
                LastLoginAt = user?.LastLoginAt
            };

            return ApiResponse<StudentProfileDto>.SuccessResponse(dto, "Student retrieved successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<StudentProfileDto>.FailureResponse($"Error retrieving student: {ex.Message}");
        }
    }

    public async Task<ApiResponse<StudentProfileDto>> GetStudentByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var student = await _unitOfWork.Students.GetByUserIdAsync(userId, cancellationToken);
            if (student == null)
                return ApiResponse<StudentProfileDto>.FailureResponse("Student not found");

            return await GetStudentByIdAsync(student.StudentID, cancellationToken);
        }
        catch (Exception ex)
        {
            return ApiResponse<StudentProfileDto>.FailureResponse($"Error retrieving student: {ex.Message}");
        }
    }

    public async Task<ApiResponse<PagedResult<StudentListDto>>> GetStudentsAsync(
        StudentFilterDto filter, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var students = await _unitOfWork.Students.GetAllAsync(cancellationToken);

            // Apply filters
            if (filter.UniversityID.HasValue)
                students = students.Where(s => s.UniversityID == filter.UniversityID.Value);

            if (filter.DepartmentID.HasValue)
                students = students.Where(s => s.DepartmentID == filter.DepartmentID.Value);

            if (!string.IsNullOrWhiteSpace(filter.AcademicYear))
            {
                if (Enum.TryParse<AcademicYear>(filter.AcademicYear, out var academicYear))
                    students = students.Where(s => s.AcademicYear == academicYear);
            }

            if (filter.MinGPA.HasValue)
                students = students.Where(s => s.AverageRating >= filter.MinGPA.Value); // Using AverageRating as proxy

            if (filter.MinRating.HasValue)
                students = students.Where(s => s.AverageRating >= filter.MinRating.Value);

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var searchLower = filter.SearchTerm.ToLower();
                students = students.Where(s =>
                    s.FirstName.ToLower().Contains(searchLower) ||
                    s.LastName.ToLower().Contains(searchLower));
            }

            if (filter.SkillID.HasValue)
            {
                var studentIdsWithSkill = (await _unitOfWork.StudentSkills.GetAllAsync(cancellationToken))
                    .Where(ss => ss.SkillID == filter.SkillID.Value)
                    .Select(ss => ss.StudentID)
                    .Distinct();
                students = students.Where(s => studentIdsWithSkill.Contains(s.StudentID));
            }

            var totalCount = students.Count();

            // Pagination
            var paginatedStudents = students
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

            var studentDtos = paginatedStudents.Select(s => new StudentListDto
            {
                StudentID = s.StudentID,
                FullName = s.FullName,
                ProfilePicture = s.ProfilePicture,
                UniversityName = s.University?.UniversityName,
                DepartmentName = s.Department?.DepartmentName,
                AcademicYear = s.AcademicYear?.ToString(),
                GPA = null, // Not in entity
                AverageRating = s.AverageRating,
                TotalReviews = s.TotalReviews,
                Skills = new List<string>() // Will populate if needed
            }).ToList();

            var result = new PagedResult<StudentListDto>
            {
                Items = studentDtos,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };

            return ApiResponse<PagedResult<StudentListDto>>.SuccessResponse(result, "Students retrieved successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<PagedResult<StudentListDto>>.FailureResponse($"Error retrieving students: {ex.Message}");
        }
    }

    public async Task<ApiResponse<StudentProfileDto>> CreateStudentAsync(CreateStudentDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate user exists
            var user = await _unitOfWork.Users.GetByIdAsync(dto.UserID, cancellationToken);
            if (user == null)
                return ApiResponse<StudentProfileDto>.FailureResponse("User not found");

            // Check if student already exists
            var existingStudent = await _unitOfWork.Students.GetByUserIdAsync(dto.UserID, cancellationToken);
            if (existingStudent != null)
                return ApiResponse<StudentProfileDto>.FailureResponse("Student profile already exists for this user");

            var student = new Student
            {
                UserID = dto.UserID,
                FirstName = user.Email.Split('@')[0], // Default from email
                LastName = string.Empty,
                Phone = dto.PhoneNumber,
                UniversityID = dto.UniversityID,
                DepartmentID = dto.DepartmentID,
                AcademicYear = !string.IsNullOrWhiteSpace(dto.AcademicYear) && Enum.TryParse<AcademicYear>(dto.AcademicYear, out var year) 
                    ? year 
                    : null,
                Country = "Unknown", // Required field
                Status = StudentStatus.Active,
                ProfileCompleteness = 0,
                AverageRating = 0,
                TotalReviews = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Students.AddAsync(student, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return await GetStudentByIdAsync(student.StudentID, cancellationToken);
        }
        catch (Exception ex)
        {
            return ApiResponse<StudentProfileDto>.FailureResponse($"Error creating student: {ex.Message}");
        }
    }

    public async Task<ApiResponse<StudentProfileDto>> UpdateStudentAsync(int studentId, UpdateStudentDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var student = await _unitOfWork.Students.GetByIdAsync(studentId, cancellationToken);
            if (student == null)
                return ApiResponse<StudentProfileDto>.FailureResponse("Student not found");

            student.Phone = dto.PhoneNumber;
            student.ProfilePicture = dto.ProfilePicture;
            student.UniversityID = dto.UniversityID;
            student.DepartmentID = dto.DepartmentID;
            if (!string.IsNullOrWhiteSpace(dto.AcademicYear) && Enum.TryParse<AcademicYear>(dto.AcademicYear, out var year))
                student.AcademicYear = year;
            student.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Students.UpdateAsync(student, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return await GetStudentByIdAsync(studentId, cancellationToken);
        }
        catch (Exception ex)
        {
            return ApiResponse<StudentProfileDto>.FailureResponse($"Error updating student: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> DeleteStudentAsync(int studentId, CancellationToken cancellationToken = default)
    {
        try
        {
            var student = await _unitOfWork.Students.GetByIdAsync(studentId, cancellationToken);
            if (student == null)
                return ApiResponse<bool>.FailureResponse("Student not found");

            await _unitOfWork.Students.DeleteAsync(studentId, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.SuccessResponse(true, "Student deleted successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.FailureResponse($"Error deleting student: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> AddSkillAsync(int studentId, int skillId, CancellationToken cancellationToken = default)
    {
        try
        {
            var student = await _unitOfWork.Students.GetByIdAsync(studentId, cancellationToken);
            if (student == null)
                return ApiResponse<bool>.FailureResponse("Student not found");

            var skill = await _unitOfWork.Skills.GetByIdAsync(skillId, cancellationToken);
            if (skill == null)
                return ApiResponse<bool>.FailureResponse("Skill not found");

            var hasSkill = await _unitOfWork.StudentSkills.StudentHasSkillAsync(studentId, skillId, cancellationToken);
            if (hasSkill)
                return ApiResponse<bool>.FailureResponse("Skill already added");

            var studentSkill = new StudentSkill
            {
                StudentID = studentId,
                SkillID = skillId,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.StudentSkills.AddAsync(studentSkill, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.SuccessResponse(true, "Skill added successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.FailureResponse($"Error adding skill: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> RemoveSkillAsync(int studentId, int skillId, CancellationToken cancellationToken = default)
    {
        try
        {
            var hasSkill = await _unitOfWork.StudentSkills.StudentHasSkillAsync(studentId, skillId, cancellationToken);
            if (!hasSkill)
                return ApiResponse<bool>.FailureResponse("Skill not found for this student");

            var skills = await _unitOfWork.StudentSkills.GetByStudentIdAsync(studentId, cancellationToken);
            var studentSkill = skills.FirstOrDefault(ss => ss.SkillID == skillId);
            if (studentSkill != null)
                await _unitOfWork.StudentSkills.DeleteAsync(studentSkill, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.SuccessResponse(true, "Skill removed successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.FailureResponse($"Error removing skill: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<SkillDto>>> GetStudentSkillsAsync(int studentId, CancellationToken cancellationToken = default)
    {
        try
        {
            var student = await _unitOfWork.Students.GetByIdAsync(studentId, cancellationToken);
            if (student == null)
                return ApiResponse<List<SkillDto>>.FailureResponse("Student not found");

            var skills = await _unitOfWork.StudentSkills.GetByStudentIdAsync(studentId, cancellationToken);

            var skillDtos = skills.Select(ss => new SkillDto
            {
                SkillID = ss.Skill.SkillID,
                SkillName = ss.Skill.SkillName,
                Category = ss.Skill.SkillCategory?.ToString()
            }).ToList();

            return ApiResponse<List<SkillDto>>.SuccessResponse(skillDtos, "Skills retrieved successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<List<SkillDto>>.FailureResponse($"Error retrieving skills: {ex.Message}");
        }
    }
}

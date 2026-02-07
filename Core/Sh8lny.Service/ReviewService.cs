using Sh8lny.Abstraction.Repositories;
using Sh8lny.Abstraction.Services;
using Sh8lny.Domain.Models;
using Sh8lny.Shared.DTOs.Common;
using Sh8lny.Shared.DTOs.Notifications;
using Sh8lny.Shared.DTOs.Reviews;

namespace Sh8lny.Service;

/// <summary>
/// Service for mutual review operations between Companies and Students.
/// </summary>
public class ReviewService : IReviewService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotifier _notifier;

    public ReviewService(IUnitOfWork unitOfWork, INotifier notifier)
    {
        _unitOfWork = unitOfWork;
        _notifier = notifier;
    }

    /// <inheritdoc />
    public async Task<ServiceResponse<bool>> ReviewStudentAsync(int companyUserId, CreateReviewDto dto)
    {
        try
        {
            // 1. Verify the company profile exists
            var company = await _unitOfWork.Companies.FindSingleAsync(c => c.UserID == companyUserId);
            if (company is null)
            {
                return ServiceResponse<bool>.Failure("Company profile not found.");
            }

            // 2. Get the application
            var application = await _unitOfWork.Applications.GetByIdAsync(dto.ApplicationId);
            if (application is null)
            {
                return ServiceResponse<bool>.Failure("Application not found.");
            }

            // 3. Verify the company owns the project
            var project = await _unitOfWork.Projects.GetByIdAsync(application.ProjectID);
            if (project is null)
            {
                return ServiceResponse<bool>.Failure("Project not found.");
            }

            if (project.CompanyID != company.CompanyID)
            {
                return ServiceResponse<bool>.Failure(
                    "You do not have permission to review this application. Only the project owner can review.");
            }

            // 4. Verify Application.Status is Completed
            if (application.Status != ApplicationStatus.Completed)
            {
                return ServiceResponse<bool>.Failure(
                    "Cannot review until the job is completed.");
            }

            // 5. Check for duplicates
            var existingReview = await _unitOfWork.StudentReviews.FindSingleAsync(
                r => r.CompanyID == company.CompanyID && 
                     r.StudentID == application.StudentID && 
                     r.ApplicationID == application.ApplicationID);

            if (existingReview is not null)
            {
                return ServiceResponse<bool>.Failure(
                    "You have already reviewed this student for this project.");
            }

            // 6. Get student info
            var student = await _unitOfWork.Students.GetByIdAsync(application.StudentID);
            if (student is null)
            {
                return ServiceResponse<bool>.Failure("Student not found.");
            }

            // 7. Create and save the review
            var review = new StudentReview
            {
                StudentID = application.StudentID,
                CompanyID = company.CompanyID,
                ProjectID = project.ProjectID,
                ApplicationID = application.ApplicationID,
                Rating = dto.Rating,
                ReviewText = dto.Comment,
                Status = ReviewStatus.Approved,
                IsPublic = true,
                IsVerified = true,
                WouldHireAgain = dto.Rating >= 4,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.StudentReviews.AddAsync(review);
            await _unitOfWork.SaveAsync();

            // 8. Recalculate student's average rating
            await RecalculateStudentRatingAsync(student);

            // 9. Send notification to the student
            var notification = new Notification
            {
                UserID = student.UserID,
                NotificationType = NotificationType.Application,
                Title = "New Review Received",
                Message = $"{company.CompanyName} has left you a {dto.Rating}-star review for '{project.ProjectName}'.",
                RelatedProjectID = project.ProjectID,
                RelatedApplicationID = application.ApplicationID,
                ActionURL = $"/profile/reviews",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Notifications.AddAsync(notification);
            await _unitOfWork.SaveAsync();

            // Send real-time notification
            var notificationDto = new NotificationDto
            {
                Id = notification.NotificationID,
                Title = notification.Title,
                Message = notification.Message,
                NotificationType = notification.NotificationType.ToString(),
                IsRead = false,
                CreatedAt = notification.CreatedAt,
                RelatedProjectId = notification.RelatedProjectID,
                RelatedApplicationId = notification.RelatedApplicationID,
                ActionUrl = notification.ActionURL
            };
            await _notifier.SendNotificationAsync(student.UserID, notificationDto);

            return ServiceResponse<bool>.Success(true, "Review submitted successfully!");
        }
        catch (Exception ex)
        {
            return ServiceResponse<bool>.Failure(
                "An error occurred while submitting the review.",
                new List<string> { ex.Message });
        }
    }

    /// <inheritdoc />
    public async Task<ServiceResponse<bool>> ReviewCompanyAsync(int studentUserId, CreateReviewDto dto)
    {
        try
        {
            // 1. Verify the student profile exists
            var student = await _unitOfWork.Students.FindSingleAsync(s => s.UserID == studentUserId);
            if (student is null)
            {
                return ServiceResponse<bool>.Failure("Student profile not found.");
            }

            // 2. Get the application
            var application = await _unitOfWork.Applications.GetByIdAsync(dto.ApplicationId);
            if (application is null)
            {
                return ServiceResponse<bool>.Failure("Application not found.");
            }

            // 3. Verify the student owns the application
            if (application.StudentID != student.StudentID)
            {
                return ServiceResponse<bool>.Failure(
                    "You do not have permission to review this company. Only the applicant can review.");
            }

            // 4. Verify Application.Status is Completed
            if (application.Status != ApplicationStatus.Completed)
            {
                return ServiceResponse<bool>.Failure(
                    "Cannot review until the job is completed.");
            }

            // 5. Get the project and company
            var project = await _unitOfWork.Projects.GetByIdAsync(application.ProjectID);
            if (project is null)
            {
                return ServiceResponse<bool>.Failure("Project not found.");
            }

            var company = await _unitOfWork.Companies.GetByIdAsync(project.CompanyID);
            if (company is null)
            {
                return ServiceResponse<bool>.Failure("Company not found.");
            }

            // 6. Check for duplicates
            var existingReview = await _unitOfWork.CompanyReviews.FindSingleAsync(
                r => r.StudentID == student.StudentID && 
                     r.CompanyID == company.CompanyID && 
                     r.ApplicationID == application.ApplicationID);

            if (existingReview is not null)
            {
                return ServiceResponse<bool>.Failure(
                    "You have already reviewed this company for this project.");
            }

            // 7. Create and save the review
            var review = new CompanyReview
            {
                CompanyID = company.CompanyID,
                StudentID = student.StudentID,
                ProjectID = project.ProjectID,
                ApplicationID = application.ApplicationID,
                Rating = dto.Rating,
                ReviewText = dto.Comment,
                Status = ReviewStatus.Approved,
                IsAnonymous = false,
                IsVerified = true,
                WouldRecommend = dto.Rating >= 4,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.CompanyReviews.AddAsync(review);
            await _unitOfWork.SaveAsync();

            // 8. Recalculate company's average rating
            await RecalculateCompanyRatingAsync(company);

            // 9. Send notification to the company
            var notification = new Notification
            {
                UserID = company.UserID,
                NotificationType = NotificationType.Application,
                Title = "New Review Received",
                Message = $"{student.FullName} has left you a {dto.Rating}-star review for '{project.ProjectName}'.",
                RelatedProjectID = project.ProjectID,
                RelatedApplicationID = application.ApplicationID,
                ActionURL = $"/company/reviews",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Notifications.AddAsync(notification);
            await _unitOfWork.SaveAsync();

            // Send real-time notification
            var notificationDto = new NotificationDto
            {
                Id = notification.NotificationID,
                Title = notification.Title,
                Message = notification.Message,
                NotificationType = notification.NotificationType.ToString(),
                IsRead = false,
                CreatedAt = notification.CreatedAt,
                RelatedProjectId = notification.RelatedProjectID,
                RelatedApplicationId = notification.RelatedApplicationID,
                ActionUrl = notification.ActionURL
            };
            await _notifier.SendNotificationAsync(company.UserID, notificationDto);

            return ServiceResponse<bool>.Success(true, "Review submitted successfully!");
        }
        catch (Exception ex)
        {
            return ServiceResponse<bool>.Failure(
                "An error occurred while submitting the review.",
                new List<string> { ex.Message });
        }
    }

    /// <inheritdoc />
    public async Task<ServiceResponse<IEnumerable<ReviewResponseDto>>> GetStudentReviewsAsync(int studentId)
    {
        try
        {
            var student = await _unitOfWork.Students.GetByIdAsync(studentId);
            if (student is null)
            {
                return ServiceResponse<IEnumerable<ReviewResponseDto>>.Failure("Student not found.");
            }

            var reviews = await _unitOfWork.StudentReviews.FindAsync(r => r.StudentID == studentId);

            var reviewDtos = new List<ReviewResponseDto>();
            foreach (var review in reviews.OrderByDescending(r => r.CreatedAt))
            {
                var company = await _unitOfWork.Companies.GetByIdAsync(review.CompanyID);
                var project = review.ProjectID.HasValue 
                    ? await _unitOfWork.Projects.GetByIdAsync(review.ProjectID.Value) 
                    : null;

                reviewDtos.Add(new ReviewResponseDto
                {
                    Id = review.ReviewID,
                    ReviewerName = company?.CompanyName ?? "Unknown Company",
                    RevieweeName = student.FullName,
                    ProjectName = project?.ProjectName ?? "Unknown Project",
                    Rating = review.Rating,
                    Comment = review.ReviewText,
                    CreatedAt = review.CreatedAt
                });
            }

            return ServiceResponse<IEnumerable<ReviewResponseDto>>.Success(reviewDtos);
        }
        catch (Exception ex)
        {
            return ServiceResponse<IEnumerable<ReviewResponseDto>>.Failure(
                "An error occurred while retrieving reviews.",
                new List<string> { ex.Message });
        }
    }

    /// <inheritdoc />
    public async Task<ServiceResponse<IEnumerable<ReviewResponseDto>>> GetCompanyReviewsAsync(int companyId)
    {
        try
        {
            var company = await _unitOfWork.Companies.GetByIdAsync(companyId);
            if (company is null)
            {
                return ServiceResponse<IEnumerable<ReviewResponseDto>>.Failure("Company not found.");
            }

            var reviews = await _unitOfWork.CompanyReviews.FindAsync(r => r.CompanyID == companyId);

            var reviewDtos = new List<ReviewResponseDto>();
            foreach (var review in reviews.OrderByDescending(r => r.CreatedAt))
            {
                var student = await _unitOfWork.Students.GetByIdAsync(review.StudentID);
                var project = review.ProjectID.HasValue 
                    ? await _unitOfWork.Projects.GetByIdAsync(review.ProjectID.Value) 
                    : null;

                reviewDtos.Add(new ReviewResponseDto
                {
                    Id = review.ReviewID,
                    ReviewerName = review.IsAnonymous ? "Anonymous" : (student?.FullName ?? "Unknown Student"),
                    RevieweeName = company.CompanyName,
                    ProjectName = project?.ProjectName ?? "Unknown Project",
                    Rating = review.Rating,
                    Comment = review.ReviewText,
                    CreatedAt = review.CreatedAt
                });
            }

            return ServiceResponse<IEnumerable<ReviewResponseDto>>.Success(reviewDtos);
        }
        catch (Exception ex)
        {
            return ServiceResponse<IEnumerable<ReviewResponseDto>>.Failure(
                "An error occurred while retrieving reviews.",
                new List<string> { ex.Message });
        }
    }

    /// <summary>
    /// Recalculates the average rating and total reviews for a student.
    /// </summary>
    private async Task RecalculateStudentRatingAsync(Student student)
    {
        var allReviews = await _unitOfWork.StudentReviews.FindAsync(r => r.StudentID == student.StudentID);
        var reviewList = allReviews.ToList();

        if (reviewList.Count > 0)
        {
            student.TotalReviews = reviewList.Count;
            student.AverageRating = Math.Round(reviewList.Average(r => r.Rating), 2);
        }
        else
        {
            student.TotalReviews = 0;
            student.AverageRating = 0;
        }

        student.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Students.Update(student);
        await _unitOfWork.SaveAsync();
    }

    /// <summary>
    /// Recalculates the average rating and total reviews for a company.
    /// </summary>
    private async Task RecalculateCompanyRatingAsync(Company company)
    {
        var allReviews = await _unitOfWork.CompanyReviews.FindAsync(r => r.CompanyID == company.CompanyID);
        var reviewList = allReviews.ToList();

        if (reviewList.Count > 0)
        {
            company.TotalReviews = reviewList.Count;
            company.AverageRating = Math.Round(reviewList.Average(r => r.Rating), 2);
        }
        else
        {
            company.TotalReviews = 0;
            company.AverageRating = 0;
        }

        company.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Companies.Update(company);
        await _unitOfWork.SaveAsync();
    }
}

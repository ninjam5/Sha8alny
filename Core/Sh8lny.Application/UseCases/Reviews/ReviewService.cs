using Sh8lny.Application.DTOs.Reviews;
using Sh8lny.Application.DTOs.Notifications;
using Sh8lny.Application.DTOs.ActivityLogs;
using Sh8lny.Application.Interfaces;
using Sh8lny.Domain.Entities;
using Sh8lny.Domain.Exceptions;
using Sh8lny.Domain.Interfaces;

namespace Sh8lny.Application.UseCases.Reviews;

/// <summary>
/// Service for review management (Company and Student reviews)
/// </summary>
public class ReviewService : IReviewService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;
    private readonly IActivityLogService _activityLogService;
    private readonly ICurrentUserService _currentUserService;

    public ReviewService(IUnitOfWork unitOfWork, INotificationService notificationService, IActivityLogService activityLogService, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
        _activityLogService = activityLogService;
        _currentUserService = currentUserService;
    }

    #region Company Review Operations

    public async Task<CompanyReviewDto> CreateCompanyReviewAsync(CreateCompanyReviewDto dto)
    {
        // Validate student exists
        var student = await _unitOfWork.Students.GetByIdAsync(dto.StudentID);
        if (student == null)
            throw new NotFoundException(nameof(Student), dto.StudentID);

        // Validate company exists
        var company = await _unitOfWork.Companies.GetByIdAsync(dto.CompanyID);
        if (company == null)
            throw new NotFoundException(nameof(Company), dto.CompanyID);

        // Check if student already reviewed this company for the same opportunity
        if (dto.CompletedOpportunityID.HasValue)
        {
            var existingReview = await _unitOfWork.CompanyReviews
                .GetByCompletedOpportunityIdAsync(dto.CompletedOpportunityID.Value);
            
            if (existingReview != null)
                throw new ValidationException("You have already reviewed this company for this opportunity");
        }

        // Validate rating
        if (dto.Rating < 1 || dto.Rating > 5)
            throw new ValidationException("Rating must be between 1 and 5");

        // Create review entity
        var review = new CompanyReview
        {
            CompanyID = dto.CompanyID,
            StudentID = dto.StudentID,
            CompletedOpportunityID = dto.CompletedOpportunityID,
            Rating = dto.Rating,
            ReviewTitle = dto.ReviewTitle,
            ReviewText = dto.ReviewText,
            WorkEnvironmentRating = dto.WorkEnvironmentRating,
            LearningOpportunityRating = dto.LearningOpportunityRating,
            MentorshipRating = dto.MentorshipRating,
            CompensationRating = dto.CompensationRating,
            CommunicationRating = dto.CommunicationRating,
            WouldRecommend = dto.WouldRecommend,
            Pros = dto.Pros,
            Cons = dto.Cons,
            IsAnonymous = dto.IsAnonymous,
            Status = ReviewStatus.Pending,
            IsVerified = dto.CompletedOpportunityID.HasValue,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.CompanyReviews.AddAsync(review);
        await _unitOfWork.SaveChangesAsync();

        // Return created review
        return await GetCompanyReviewByIdAsync(review.ReviewID);
    }

    public async Task<CompanyReviewDto> GetCompanyReviewByIdAsync(int reviewId)
    {
        var review = await _unitOfWork.CompanyReviews.GetByIdAsync(reviewId);
        if (review == null)
            throw new NotFoundException(nameof(CompanyReview), reviewId);

        // Load related entities
        var company = await _unitOfWork.Companies.GetByIdAsync(review.CompanyID);
        var student = await _unitOfWork.Students.GetByIdAsync(review.StudentID);

        return MapToCompanyReviewDto(review, company, student);
    }

    public async Task<IEnumerable<CompanyReviewDto>> GetCompanyReviewsAsync(int companyId, int page = 1, int pageSize = 20)
    {
        // Validate company exists
        var company = await _unitOfWork.Companies.GetByIdAsync(companyId);
        if (company == null)
            throw new NotFoundException(nameof(Company), companyId);

        // Get approved reviews only for public viewing
        var reviews = await _unitOfWork.CompanyReviews.GetApprovedReviewsAsync(companyId);
        
        // Apply pagination
        var paginatedReviews = reviews
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var reviewDtos = new List<CompanyReviewDto>();
        foreach (var review in paginatedReviews)
        {
            var student = await _unitOfWork.Students.GetByIdAsync(review.StudentID);
            reviewDtos.Add(MapToCompanyReviewDto(review, company, student));
        }

        return reviewDtos;
    }

    public async Task<IEnumerable<CompanyReviewDto>> GetStudentCompanyReviewsAsync(int studentId)
    {
        // Validate student exists
        var student = await _unitOfWork.Students.GetByIdAsync(studentId);
        if (student == null)
            throw new NotFoundException(nameof(Student), studentId);

        // Get all reviews written by this student
        var reviews = await _unitOfWork.CompanyReviews.GetByStudentIdAsync(studentId);
        
        var reviewDtos = new List<CompanyReviewDto>();
        foreach (var review in reviews.OrderByDescending(r => r.CreatedAt))
        {
            var company = await _unitOfWork.Companies.GetByIdAsync(review.CompanyID);
            reviewDtos.Add(MapToCompanyReviewDto(review, company, student));
        }

        return reviewDtos;
    }

    public async Task<CompanyReviewDto> UpdateCompanyReviewAsync(UpdateCompanyReviewDto dto, int studentId)
    {
        var review = await _unitOfWork.CompanyReviews.GetByIdAsync(dto.ReviewID);
        if (review == null)
            throw new NotFoundException(nameof(CompanyReview), dto.ReviewID);

        // Verify ownership
        if (review.StudentID != studentId)
            throw new UnauthorizedException("You are not authorized to update this review");

        // Update only provided fields
        if (dto.Rating.HasValue)
        {
            if (dto.Rating < 1 || dto.Rating > 5)
                throw new ValidationException("Rating must be between 1 and 5");
            review.Rating = dto.Rating.Value;
        }

        if (!string.IsNullOrWhiteSpace(dto.ReviewTitle))
            review.ReviewTitle = dto.ReviewTitle;

        if (!string.IsNullOrWhiteSpace(dto.ReviewText))
            review.ReviewText = dto.ReviewText;

        if (dto.WorkEnvironmentRating.HasValue)
            review.WorkEnvironmentRating = dto.WorkEnvironmentRating;

        if (dto.LearningOpportunityRating.HasValue)
            review.LearningOpportunityRating = dto.LearningOpportunityRating;

        if (dto.MentorshipRating.HasValue)
            review.MentorshipRating = dto.MentorshipRating;

        if (dto.CompensationRating.HasValue)
            review.CompensationRating = dto.CompensationRating;

        if (dto.CommunicationRating.HasValue)
            review.CommunicationRating = dto.CommunicationRating;

        if (dto.WouldRecommend.HasValue)
            review.WouldRecommend = dto.WouldRecommend.Value;

        if (dto.Pros != null)
            review.Pros = dto.Pros;

        if (dto.Cons != null)
            review.Cons = dto.Cons;

        review.UpdatedAt = DateTime.UtcNow;
        review.Status = ReviewStatus.Pending; // Reset to pending after update

        await _unitOfWork.CompanyReviews.UpdateAsync(review);
        await _unitOfWork.SaveChangesAsync();

        // Recalculate company average rating
        await RecalculateCompanyRatingAsync(review.CompanyID);

        return await GetCompanyReviewByIdAsync(review.ReviewID);
    }

    public async Task<bool> DeleteCompanyReviewAsync(int reviewId, int studentId)
    {
        var review = await _unitOfWork.CompanyReviews.GetByIdAsync(reviewId);
        if (review == null)
            throw new NotFoundException(nameof(CompanyReview), reviewId);

        // Verify ownership
        if (review.StudentID != studentId)
            throw new UnauthorizedException("You are not authorized to delete this review");

        var companyId = review.CompanyID;

        await _unitOfWork.CompanyReviews.DeleteAsync(review);
        await _unitOfWork.SaveChangesAsync();

        // Recalculate company average rating
        await RecalculateCompanyRatingAsync(companyId);

        return true;
    }

    public async Task<CompanyReviewDto> AddCompanyResponseAsync(CompanyResponseDto dto, int companyId)
    {
        var review = await _unitOfWork.CompanyReviews.GetByIdAsync(dto.ReviewID);
        if (review == null)
            throw new NotFoundException(nameof(CompanyReview), dto.ReviewID);

        // Verify company ownership
        if (review.CompanyID != companyId)
            throw new UnauthorizedException("You are not authorized to respond to this review");

        // Validate review is approved
        if (review.Status != ReviewStatus.Approved)
            throw new ValidationException("Cannot respond to a review that is not approved");

        // Add company response
        review.CompanyResponse = dto.CompanyResponse;
        review.CompanyRespondedAt = DateTime.UtcNow;

        await _unitOfWork.CompanyReviews.UpdateAsync(review);
        await _unitOfWork.SaveChangesAsync();

        // Load related entities for notification
        var company = await _unitOfWork.Companies.GetByIdAsync(review.CompanyID);
        var student = await _unitOfWork.Students.GetByIdAsync(review.StudentID);

        // Notify the review author (student) that the company responded
        if (student != null && company != null)
        {
            await _notificationService.CreateNotificationAsync(new CreateNotificationDto
            {
                UserID = student.UserID,
                NotificationType = (int)NotificationType.Message,
                Title = "Company Response to Your Review",
                Message = $"{company.CompanyName} has responded to your review.",
                RelatedProjectID = null,
                RelatedApplicationID = null,
                ActionURL = $"/reviews/companies/{review.ReviewID}"
            });
        }

        return await GetCompanyReviewByIdAsync(review.ReviewID);
    }

    public async Task<ReviewStatsDto> GetCompanyReviewStatsAsync(int companyId)
    {
        // Validate company exists
        var company = await _unitOfWork.Companies.GetByIdAsync(companyId);
        if (company == null)
            throw new NotFoundException(nameof(Company), companyId);

        // Get all approved reviews
        var reviews = await _unitOfWork.CompanyReviews.GetApprovedReviewsAsync(companyId);
        var reviewList = reviews.ToList();

        if (!reviewList.Any())
        {
            return new ReviewStatsDto
            {
                TotalReviews = 0,
                AverageRating = 0,
                FiveStarCount = 0,
                FourStarCount = 0,
                ThreeStarCount = 0,
                TwoStarCount = 0,
                OneStarCount = 0,
                RecommendationPercentage = 0
            };
        }

        // Calculate statistics
        var totalReviews = reviewList.Count;
        var averageRating = reviewList.Average(r => r.Rating);
        var fiveStarCount = reviewList.Count(r => r.Rating >= 4.5m);
        var fourStarCount = reviewList.Count(r => r.Rating >= 3.5m && r.Rating < 4.5m);
        var threeStarCount = reviewList.Count(r => r.Rating >= 2.5m && r.Rating < 3.5m);
        var twoStarCount = reviewList.Count(r => r.Rating >= 1.5m && r.Rating < 2.5m);
        var oneStarCount = reviewList.Count(r => r.Rating < 1.5m);
        var recommendCount = reviewList.Count(r => r.WouldRecommend);
        var recommendationPercentage = (int)Math.Round((decimal)recommendCount / totalReviews * 100);

        return new ReviewStatsDto
        {
            TotalReviews = totalReviews,
            AverageRating = Math.Round(averageRating, 2),
            FiveStarCount = fiveStarCount,
            FourStarCount = fourStarCount,
            ThreeStarCount = threeStarCount,
            TwoStarCount = twoStarCount,
            OneStarCount = oneStarCount,
            RecommendationPercentage = recommendationPercentage
        };
    }

    #endregion

    #region Student Review Operations

    public async Task<StudentReviewDto> CreateStudentReviewAsync(CreateStudentReviewDto dto)
    {
        // Validate student exists
        var student = await _unitOfWork.Students.GetByIdAsync(dto.StudentID);
        if (student == null)
            throw new NotFoundException(nameof(Student), dto.StudentID);

        // Validate company exists
        var company = await _unitOfWork.Companies.GetByIdAsync(dto.CompanyID);
        if (company == null)
            throw new NotFoundException(nameof(Company), dto.CompanyID);

        // Check if company already reviewed this student for the same opportunity
        if (dto.CompletedOpportunityID.HasValue)
        {
            var existingReview = await _unitOfWork.StudentReviews
                .GetByCompletedOpportunityIdAsync(dto.CompletedOpportunityID.Value);
            
            if (existingReview != null)
                throw new ValidationException("You have already reviewed this student for this opportunity");
        }

        // Validate rating
        if (dto.Rating < 1 || dto.Rating > 5)
            throw new ValidationException("Rating must be between 1 and 5");

        // Create review entity
        var review = new StudentReview
        {
            StudentID = dto.StudentID,
            CompanyID = dto.CompanyID,
            CompletedOpportunityID = dto.CompletedOpportunityID,
            Rating = dto.Rating,
            ReviewTitle = dto.ReviewTitle,
            ReviewText = dto.ReviewText,
            TechnicalSkillsRating = dto.TechnicalSkillsRating,
            CommunicationRating = dto.CommunicationRating,
            ProfessionalismRating = dto.ProfessionalismRating,
            TimeManagementRating = dto.TimeManagementRating,
            TeamworkRating = dto.TeamworkRating,
            ProblemSolvingRating = dto.ProblemSolvingRating,
            WouldHireAgain = dto.WouldHireAgain,
            Strengths = dto.Strengths,
            AreasForImprovement = dto.AreasForImprovement,
            IsPublic = dto.IsPublic,
            Status = ReviewStatus.Pending,
            IsVerified = dto.CompletedOpportunityID.HasValue,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.StudentReviews.AddAsync(review);
        await _unitOfWork.SaveChangesAsync();

        // Return created review
        return await GetStudentReviewByIdAsync(review.ReviewID);
    }

    public async Task<StudentReviewDto> GetStudentReviewByIdAsync(int reviewId)
    {
        var review = await _unitOfWork.StudentReviews.GetByIdAsync(reviewId);
        if (review == null)
            throw new NotFoundException(nameof(StudentReview), reviewId);

        // Load related entities
        var student = await _unitOfWork.Students.GetByIdAsync(review.StudentID);
        var company = await _unitOfWork.Companies.GetByIdAsync(review.CompanyID);

        return MapToStudentReviewDto(review, student, company);
    }

    public async Task<IEnumerable<StudentReviewDto>> GetStudentReviewsAsync(int studentId, int page = 1, int pageSize = 20)
    {
        // Validate student exists
        var student = await _unitOfWork.Students.GetByIdAsync(studentId);
        if (student == null)
            throw new NotFoundException(nameof(Student), studentId);

        // Get approved and public reviews only for public viewing
        var reviews = await _unitOfWork.StudentReviews.GetPublicReviewsAsync(studentId);
        
        // Apply pagination
        var paginatedReviews = reviews
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var reviewDtos = new List<StudentReviewDto>();
        foreach (var review in paginatedReviews)
        {
            var company = await _unitOfWork.Companies.GetByIdAsync(review.CompanyID);
            reviewDtos.Add(MapToStudentReviewDto(review, student, company));
        }

        return reviewDtos;
    }

    public async Task<IEnumerable<StudentReviewDto>> GetCompanyStudentReviewsAsync(int companyId)
    {
        // Validate company exists
        var company = await _unitOfWork.Companies.GetByIdAsync(companyId);
        if (company == null)
            throw new NotFoundException(nameof(Company), companyId);

        // Get all reviews written by this company
        var reviews = await _unitOfWork.StudentReviews.GetByCompanyIdAsync(companyId);
        
        var reviewDtos = new List<StudentReviewDto>();
        foreach (var review in reviews.OrderByDescending(r => r.CreatedAt))
        {
            var student = await _unitOfWork.Students.GetByIdAsync(review.StudentID);
            reviewDtos.Add(MapToStudentReviewDto(review, student, company));
        }

        return reviewDtos;
    }

    public async Task<StudentReviewDto> UpdateStudentReviewAsync(UpdateStudentReviewDto dto, int companyId)
    {
        var review = await _unitOfWork.StudentReviews.GetByIdAsync(dto.ReviewID);
        if (review == null)
            throw new NotFoundException(nameof(StudentReview), dto.ReviewID);

        // Verify ownership
        if (review.CompanyID != companyId)
            throw new UnauthorizedException("You are not authorized to update this review");

        // Update only provided fields
        if (dto.Rating.HasValue)
        {
            if (dto.Rating < 1 || dto.Rating > 5)
                throw new ValidationException("Rating must be between 1 and 5");
            review.Rating = dto.Rating.Value;
        }

        if (!string.IsNullOrWhiteSpace(dto.ReviewTitle))
            review.ReviewTitle = dto.ReviewTitle;

        if (!string.IsNullOrWhiteSpace(dto.ReviewText))
            review.ReviewText = dto.ReviewText;

        if (dto.TechnicalSkillsRating.HasValue)
            review.TechnicalSkillsRating = dto.TechnicalSkillsRating;

        if (dto.CommunicationRating.HasValue)
            review.CommunicationRating = dto.CommunicationRating;

        if (dto.ProfessionalismRating.HasValue)
            review.ProfessionalismRating = dto.ProfessionalismRating;

        if (dto.TimeManagementRating.HasValue)
            review.TimeManagementRating = dto.TimeManagementRating;

        if (dto.TeamworkRating.HasValue)
            review.TeamworkRating = dto.TeamworkRating;

        if (dto.ProblemSolvingRating.HasValue)
            review.ProblemSolvingRating = dto.ProblemSolvingRating;

        if (dto.WouldHireAgain.HasValue)
            review.WouldHireAgain = dto.WouldHireAgain.Value;

        if (dto.Strengths != null)
            review.Strengths = dto.Strengths;

        if (dto.AreasForImprovement != null)
            review.AreasForImprovement = dto.AreasForImprovement;

        if (dto.IsPublic.HasValue)
            review.IsPublic = dto.IsPublic.Value;

        review.UpdatedAt = DateTime.UtcNow;
        review.Status = ReviewStatus.Pending; // Reset to pending after update

        await _unitOfWork.StudentReviews.UpdateAsync(review);
        await _unitOfWork.SaveChangesAsync();

        // Recalculate student average rating
        await RecalculateStudentRatingAsync(review.StudentID);

        return await GetStudentReviewByIdAsync(review.ReviewID);
    }

    public async Task<bool> DeleteStudentReviewAsync(int reviewId, int companyId)
    {
        var review = await _unitOfWork.StudentReviews.GetByIdAsync(reviewId);
        if (review == null)
            throw new NotFoundException(nameof(StudentReview), reviewId);

        // Verify ownership
        if (review.CompanyID != companyId)
            throw new UnauthorizedException("You are not authorized to delete this review");

        var studentId = review.StudentID;

        await _unitOfWork.StudentReviews.DeleteAsync(review);
        await _unitOfWork.SaveChangesAsync();

        // Recalculate student average rating
        await RecalculateStudentRatingAsync(studentId);

        return true;
    }

    public async Task<StudentReviewDto> AddStudentResponseAsync(StudentResponseDto dto, int studentId)
    {
        var review = await _unitOfWork.StudentReviews.GetByIdAsync(dto.ReviewID);
        if (review == null)
            throw new NotFoundException(nameof(StudentReview), dto.ReviewID);

        // Verify student ownership
        if (review.StudentID != studentId)
            throw new UnauthorizedException("You are not authorized to respond to this review");

        // Validate review is approved
        if (review.Status != ReviewStatus.Approved)
            throw new ValidationException("Cannot respond to a review that is not approved");

        // Add student response
        review.StudentResponse = dto.StudentResponse;
        review.StudentRespondedAt = DateTime.UtcNow;

        await _unitOfWork.StudentReviews.UpdateAsync(review);
        await _unitOfWork.SaveChangesAsync();

        // Load related entities for notification
        var student = await _unitOfWork.Students.GetByIdAsync(review.StudentID);
        var company = await _unitOfWork.Companies.GetByIdAsync(review.CompanyID);

        // Notify the review author (company) that the student responded
        if (company != null && student != null)
        {
            await _notificationService.CreateNotificationAsync(new CreateNotificationDto
            {
                UserID = company.UserID,
                NotificationType = (int)NotificationType.Message,
                Title = "Student Response to Your Review",
                Message = $"{student.FirstName} {student.LastName} has responded to your review.",
                RelatedProjectID = null,
                RelatedApplicationID = null,
                ActionURL = $"/reviews/students/{review.ReviewID}"
            });
        }

        return await GetStudentReviewByIdAsync(review.ReviewID);
    }

    public async Task<ReviewStatsDto> GetStudentReviewStatsAsync(int studentId)
    {
        // Validate student exists
        var student = await _unitOfWork.Students.GetByIdAsync(studentId);
        if (student == null)
            throw new NotFoundException(nameof(Student), studentId);

        // Get all approved and public reviews
        var reviews = await _unitOfWork.StudentReviews.GetPublicReviewsAsync(studentId);
        var reviewList = reviews.ToList();

        if (!reviewList.Any())
        {
            return new ReviewStatsDto
            {
                TotalReviews = 0,
                AverageRating = 0,
                FiveStarCount = 0,
                FourStarCount = 0,
                ThreeStarCount = 0,
                TwoStarCount = 0,
                OneStarCount = 0,
                RecommendationPercentage = 0
            };
        }

        // Calculate statistics
        var totalReviews = reviewList.Count;
        var averageRating = reviewList.Average(r => r.Rating);
        var fiveStarCount = reviewList.Count(r => r.Rating >= 4.5m);
        var fourStarCount = reviewList.Count(r => r.Rating >= 3.5m && r.Rating < 4.5m);
        var threeStarCount = reviewList.Count(r => r.Rating >= 2.5m && r.Rating < 3.5m);
        var twoStarCount = reviewList.Count(r => r.Rating >= 1.5m && r.Rating < 2.5m);
        var oneStarCount = reviewList.Count(r => r.Rating < 1.5m);
        var hireAgainCount = reviewList.Count(r => r.WouldHireAgain);
        var recommendationPercentage = (int)Math.Round((decimal)hireAgainCount / totalReviews * 100);

        return new ReviewStatsDto
        {
            TotalReviews = totalReviews,
            AverageRating = Math.Round(averageRating, 2),
            FiveStarCount = fiveStarCount,
            FourStarCount = fourStarCount,
            ThreeStarCount = threeStarCount,
            TwoStarCount = twoStarCount,
            OneStarCount = oneStarCount,
            RecommendationPercentage = recommendationPercentage
        };
    }

    #endregion

    #region Review Moderation

    public async Task<bool> ApproveReviewAsync(int reviewId, string reviewType)
    {
        // Validate review type
        if (reviewType.ToLower() != "company" && reviewType.ToLower() != "student")
            throw new ValidationException("Invalid review type. Must be 'company' or 'student'");

        if (reviewType.ToLower() == "company")
        {
            var review = await _unitOfWork.CompanyReviews.GetByIdAsync(reviewId);
            if (review == null)
                throw new NotFoundException(nameof(CompanyReview), reviewId);

            review.Status = ReviewStatus.Approved;
            await _unitOfWork.CompanyReviews.UpdateAsync(review);
            await _unitOfWork.SaveChangesAsync();

            // Log admin's action
            await _activityLogService.CreateActivityLogAsync(new CreateActivityLogDto
            {
                UserID = _currentUserService.UserId,
                ActivityType = "ReviewApproved",
                Description = $"Admin approved company review {review.ReviewID}",
                RelatedEntityType = "CompanyReview",
                RelatedEntityID = review.ReviewID
            });

            // Recalculate company rating after approval
            await RecalculateCompanyRatingAsync(review.CompanyID);

            // Load related entities for notifications
            var company = await _unitOfWork.Companies.GetByIdAsync(review.CompanyID);
            var student = await _unitOfWork.Students.GetByIdAsync(review.StudentID);

            // Notify the review author (student) that their review is now live
            if (student != null && company != null)
            {
                await _notificationService.CreateNotificationAsync(new CreateNotificationDto
                {
                    UserID = student.UserID,
                    NotificationType = (int)NotificationType.System,
                    Title = "Review Approved",
                    Message = $"Your review for {company.CompanyName} is now public.",
                    RelatedProjectID = null,
                    RelatedApplicationID = null,
                    ActionURL = $"/reviews/companies/{review.ReviewID}"
                });
            }

            // Notify the company that they received a new review
            if (company != null)
            {
                var reviewerName = review.IsAnonymous ? "Anonymous" : (student != null ? $"{student.FirstName} {student.LastName}" : "A student");
                await _notificationService.CreateNotificationAsync(new CreateNotificationDto
                {
                    UserID = company.UserID,
                    NotificationType = (int)NotificationType.System,
                    Title = "New Review Received",
                    Message = $"You have a new review from {reviewerName}.",
                    RelatedProjectID = null,
                    RelatedApplicationID = null,
                    ActionURL = $"/reviews/companies/{review.ReviewID}"
                });
            }
        }
        else
        {
            var review = await _unitOfWork.StudentReviews.GetByIdAsync(reviewId);
            if (review == null)
                throw new NotFoundException(nameof(StudentReview), reviewId);

            review.Status = ReviewStatus.Approved;
            await _unitOfWork.StudentReviews.UpdateAsync(review);
            await _unitOfWork.SaveChangesAsync();

            // Log admin's action
            await _activityLogService.CreateActivityLogAsync(new CreateActivityLogDto
            {
                UserID = _currentUserService.UserId,
                ActivityType = "ReviewApproved",
                Description = $"Admin approved student review {review.ReviewID}",
                RelatedEntityType = "StudentReview",
                RelatedEntityID = review.ReviewID
            });

            // Recalculate student rating after approval
            await RecalculateStudentRatingAsync(review.StudentID);

            // Load related entities for notifications
            var student = await _unitOfWork.Students.GetByIdAsync(review.StudentID);
            var company = await _unitOfWork.Companies.GetByIdAsync(review.CompanyID);

            // Notify the review author (company) that their review is now live
            if (company != null && student != null)
            {
                await _notificationService.CreateNotificationAsync(new CreateNotificationDto
                {
                    UserID = company.UserID,
                    NotificationType = (int)NotificationType.System,
                    Title = "Review Approved",
                    Message = $"Your review for {student.FirstName} {student.LastName} is now public.",
                    RelatedProjectID = null,
                    RelatedApplicationID = null,
                    ActionURL = $"/reviews/students/{review.ReviewID}"
                });
            }

            // Notify the student that they received a new review
            if (student != null && company != null)
            {
                await _notificationService.CreateNotificationAsync(new CreateNotificationDto
                {
                    UserID = student.UserID,
                    NotificationType = (int)NotificationType.System,
                    Title = "New Review Received",
                    Message = $"You have a new review from {company.CompanyName}.",
                    RelatedProjectID = null,
                    RelatedApplicationID = null,
                    ActionURL = $"/reviews/students/{review.ReviewID}"
                });
            }
        }

        return true;
    }

    public async Task<bool> RejectReviewAsync(int reviewId, string reviewType)
    {
        // Validate review type
        if (reviewType.ToLower() != "company" && reviewType.ToLower() != "student")
            throw new ValidationException("Invalid review type. Must be 'company' or 'student'");

        if (reviewType.ToLower() == "company")
        {
            var review = await _unitOfWork.CompanyReviews.GetByIdAsync(reviewId);
            if (review == null)
                throw new NotFoundException(nameof(CompanyReview), reviewId);

            review.Status = ReviewStatus.Rejected;
            await _unitOfWork.CompanyReviews.UpdateAsync(review);
            await _unitOfWork.SaveChangesAsync();

            // Log admin's action
            await _activityLogService.CreateActivityLogAsync(new CreateActivityLogDto
            {
                UserID = _currentUserService.UserId,
                ActivityType = "ReviewRejected",
                Description = $"Admin rejected company review {review.ReviewID}",
                RelatedEntityType = "CompanyReview",
                RelatedEntityID = review.ReviewID
            });

            // Recalculate company rating after rejection
            await RecalculateCompanyRatingAsync(review.CompanyID);
        }
        else
        {
            var review = await _unitOfWork.StudentReviews.GetByIdAsync(reviewId);
            if (review == null)
                throw new NotFoundException(nameof(StudentReview), reviewId);

            review.Status = ReviewStatus.Rejected;
            await _unitOfWork.StudentReviews.UpdateAsync(review);
            await _unitOfWork.SaveChangesAsync();

            // Log admin's action
            await _activityLogService.CreateActivityLogAsync(new CreateActivityLogDto
            {
                UserID = _currentUserService.UserId,
                ActivityType = "ReviewRejected",
                Description = $"Admin rejected student review {review.ReviewID}",
                RelatedEntityType = "StudentReview",
                RelatedEntityID = review.ReviewID
            });

            // Recalculate student rating after rejection
            await RecalculateStudentRatingAsync(review.StudentID);
        }

        return true;
    }

    public async Task<bool> FlagReviewAsync(int reviewId, string reviewType, int reportingUserId)
    {
        // Validate review type
        if (reviewType.ToLower() != "company" && reviewType.ToLower() != "student")
            throw new ValidationException("Invalid review type. Must be 'company' or 'student'");

        if (reviewType.ToLower() == "company")
        {
            var review = await _unitOfWork.CompanyReviews.GetByIdAsync(reviewId);
            if (review == null)
                throw new NotFoundException(nameof(CompanyReview), reviewId);

            // Only flag approved reviews
            if (review.Status != ReviewStatus.Approved)
                throw new ValidationException("Can only flag approved reviews");

            review.Status = ReviewStatus.Flagged;
            await _unitOfWork.CompanyReviews.UpdateAsync(review);
        }
        else
        {
            var review = await _unitOfWork.StudentReviews.GetByIdAsync(reviewId);
            if (review == null)
                throw new NotFoundException(nameof(StudentReview), reviewId);

            // Only flag approved reviews
            if (review.Status != ReviewStatus.Approved)
                throw new ValidationException("Can only flag approved reviews");

            review.Status = ReviewStatus.Flagged;
            await _unitOfWork.StudentReviews.UpdateAsync(review);
        }

        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Maps CompanyReview entity to DTO
    /// </summary>
    private CompanyReviewDto MapToCompanyReviewDto(CompanyReview review, Company? company, Student? student)
    {
        return new CompanyReviewDto
        {
            ReviewID = review.ReviewID,
            CompanyID = review.CompanyID,
            CompanyName = company?.CompanyName ?? "Unknown Company",
            StudentID = review.StudentID,
            StudentName = review.IsAnonymous ? null : $"{student?.FirstName} {student?.LastName}",
            CompletedOpportunityID = review.CompletedOpportunityID,
            Rating = review.Rating,
            ReviewTitle = review.ReviewTitle,
            ReviewText = review.ReviewText,
            WorkEnvironmentRating = review.WorkEnvironmentRating,
            LearningOpportunityRating = review.LearningOpportunityRating,
            MentorshipRating = review.MentorshipRating,
            CompensationRating = review.CompensationRating,
            CommunicationRating = review.CommunicationRating,
            WouldRecommend = review.WouldRecommend,
            Pros = review.Pros,
            Cons = review.Cons,
            Status = review.Status.ToString(),
            IsVerified = review.IsVerified,
            IsAnonymous = review.IsAnonymous,
            CreatedAt = review.CreatedAt,
            UpdatedAt = review.UpdatedAt,
            CompanyResponse = review.CompanyResponse,
            CompanyRespondedAt = review.CompanyRespondedAt
        };
    }

    /// <summary>
    /// Maps StudentReview entity to DTO
    /// </summary>
    private StudentReviewDto MapToStudentReviewDto(StudentReview review, Student? student, Company? company)
    {
        return new StudentReviewDto
        {
            ReviewID = review.ReviewID,
            StudentID = review.StudentID,
            StudentName = $"{student?.FirstName} {student?.LastName}",
            CompanyID = review.CompanyID,
            CompanyName = company?.CompanyName ?? "Unknown Company",
            CompletedOpportunityID = review.CompletedOpportunityID,
            Rating = review.Rating,
            ReviewTitle = review.ReviewTitle,
            ReviewText = review.ReviewText,
            TechnicalSkillsRating = review.TechnicalSkillsRating,
            CommunicationRating = review.CommunicationRating,
            ProfessionalismRating = review.ProfessionalismRating,
            TimeManagementRating = review.TimeManagementRating,
            TeamworkRating = review.TeamworkRating,
            ProblemSolvingRating = review.ProblemSolvingRating,
            WouldHireAgain = review.WouldHireAgain,
            Strengths = review.Strengths,
            AreasForImprovement = review.AreasForImprovement,
            Status = review.Status.ToString(),
            IsVerified = review.IsVerified,
            IsPublic = review.IsPublic,
            CreatedAt = review.CreatedAt,
            UpdatedAt = review.UpdatedAt,
            StudentResponse = review.StudentResponse,
            StudentRespondedAt = review.StudentRespondedAt
        };
    }

    /// <summary>
    /// Recalculates and updates company average rating
    /// </summary>
    private async Task RecalculateCompanyRatingAsync(int companyId)
    {
        var company = await _unitOfWork.Companies.GetByIdAsync(companyId);
        if (company == null)
            return;

        // Get approved reviews only
        var reviews = await _unitOfWork.CompanyReviews.GetApprovedReviewsAsync(companyId);
        var reviewList = reviews.ToList();

        if (reviewList.Any())
        {
            company.AverageRating = Math.Round(reviewList.Average(r => r.Rating), 2);
            company.TotalReviews = reviewList.Count;
        }
        else
        {
            company.AverageRating = 0;
            company.TotalReviews = 0;
        }

        company.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.Companies.UpdateAsync(company);
        await _unitOfWork.SaveChangesAsync();
    }

    /// <summary>
    /// Recalculates and updates student average rating
    /// </summary>
    private async Task RecalculateStudentRatingAsync(int studentId)
    {
        var student = await _unitOfWork.Students.GetByIdAsync(studentId);
        if (student == null)
            return;

        // Get approved and public reviews only
        var reviews = await _unitOfWork.StudentReviews.GetPublicReviewsAsync(studentId);
        var reviewList = reviews.ToList();

        if (reviewList.Any())
        {
            student.AverageRating = Math.Round(reviewList.Average(r => r.Rating), 2);
            student.TotalReviews = reviewList.Count;
        }
        else
        {
            student.AverageRating = 0;
            student.TotalReviews = 0;
        }

        student.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.Students.UpdateAsync(student);
        await _unitOfWork.SaveChangesAsync();
    }

    #endregion
}

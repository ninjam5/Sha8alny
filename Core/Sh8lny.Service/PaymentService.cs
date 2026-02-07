using Sh8lny.Abstraction.Repositories;
using Sh8lny.Abstraction.Services;
using Sh8lny.Domain.Models;
using Sh8lny.Shared.DTOs.Common;
using Sh8lny.Shared.DTOs.Notifications;
using Sh8lny.Shared.DTOs.Payments;

namespace Sh8lny.Service;

/// <summary>
/// Service for processing payments between Companies and Students.
/// This is a mock implementation for simulation purposes.
/// </summary>
public class PaymentService : IPaymentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotifier _notifier;

    public PaymentService(IUnitOfWork unitOfWork, INotifier notifier)
    {
        _unitOfWork = unitOfWork;
        _notifier = notifier;
    }

    /// <inheritdoc />
    public async Task<ServiceResponse<PaymentReceiptDto>> ProcessPaymentAsync(int companyUserId, ProcessPaymentDto dto)
    {
        try
        {
            // 1. Verify the company profile exists
            var company = await _unitOfWork.Companies.FindSingleAsync(c => c.UserID == companyUserId);
            if (company is null)
            {
                return ServiceResponse<PaymentReceiptDto>.Failure("Company profile not found.");
            }

            // 2. Get the application
            var application = await _unitOfWork.Applications.GetByIdAsync(dto.ApplicationId);
            if (application is null)
            {
                return ServiceResponse<PaymentReceiptDto>.Failure("Application not found.");
            }

            // 3. Verify the company owns the project (Security check)
            var project = await _unitOfWork.Projects.GetByIdAsync(application.ProjectID);
            if (project is null)
            {
                return ServiceResponse<PaymentReceiptDto>.Failure("Project not found.");
            }

            if (project.CompanyID != company.CompanyID)
            {
                return ServiceResponse<PaymentReceiptDto>.Failure(
                    "You do not have permission to process payment for this application.");
            }

            // 4. CRUCIAL: Verify Application.Status is Completed
            if (application.Status != ApplicationStatus.Completed)
            {
                return ServiceResponse<PaymentReceiptDto>.Failure(
                    $"Cannot process payment for application with status '{application.Status}'. Application must be Completed.");
            }

            // 5. Check if already paid (Prevent double charging)
            if (application.IsPaid)
            {
                return ServiceResponse<PaymentReceiptDto>.Failure(
                    "Payment has already been processed for this application.");
            }

            // 6. Get student info
            var student = await _unitOfWork.Students.GetByIdAsync(application.StudentID);
            if (student is null)
            {
                return ServiceResponse<PaymentReceiptDto>.Failure("Student not found.");
            }

            // 7. Mock Payment Processing
            // Simulate processing delay
            await Task.Delay(500);

            // Optional: Test failure scenario
            if (dto.PaymentMethod.Equals("FailTest", StringComparison.OrdinalIgnoreCase))
            {
                return ServiceResponse<PaymentReceiptDto>.Failure(
                    "Payment failed. Please try again or use a different payment method.");
            }

            // 8. Calculate amount (use BidAmount or a default)
            var amount = application.BidAmount ?? 0;

            // 9. Create Transaction record
            var transaction = new Transaction
            {
                ApplicationId = application.ApplicationID,
                PayerId = companyUserId,
                PayeeId = student.UserID,
                Amount = amount,
                TransactionDate = DateTime.UtcNow,
                PaymentMethod = dto.PaymentMethod,
                ReferenceId = Guid.NewGuid().ToString("N").ToUpper(),
                Status = TransactionStatus.Completed
            };

            await _unitOfWork.Transactions.AddAsync(transaction);

            // 10. Update Application payment status
            application.IsPaid = true;
            application.PaidAt = DateTime.UtcNow;
            _unitOfWork.Applications.Update(application);

            await _unitOfWork.SaveAsync();

            // 11. Send notification to the student
            var notification = new Notification
            {
                UserID = student.UserID,
                NotificationType = NotificationType.Payment,
                Title = "Payment Received!",
                Message = $"You have received a payment of ${amount:N2} for project '{project.ProjectName}'.",
                RelatedProjectID = project.ProjectID,
                RelatedApplicationID = application.ApplicationID,
                ActionURL = $"/payments/{transaction.Id}",
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

            // 12. Build and return receipt
            var receipt = new PaymentReceiptDto
            {
                TransactionId = transaction.Id,
                ReferenceId = transaction.ReferenceId,
                Amount = amount,
                Currency = "USD",
                Date = transaction.TransactionDate,
                PaymentMethod = dto.PaymentMethod,
                PayerName = company.CompanyName ?? "Unknown Company",
                PayeeName = student.FullName ?? "Unknown Student",
                ProjectName = project.ProjectName,
                Message = "Payment Successful"
            };

            return ServiceResponse<PaymentReceiptDto>.Success(receipt, "Payment processed successfully!");
        }
        catch (Exception ex)
        {
            return ServiceResponse<PaymentReceiptDto>.Failure(
                "An error occurred while processing the payment.",
                new List<string> { ex.Message });
        }
    }
}

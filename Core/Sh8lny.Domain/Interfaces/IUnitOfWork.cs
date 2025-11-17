using Sh8lny.Domain.Interfaces.Repositories;

namespace Sh8lny.Domain.Interfaces;

/// <summary>
/// Unit of Work pattern interface for managing transactions and repositories
/// </summary>
public interface IUnitOfWork : IDisposable
{
    // Repository properties for each entity
    IUserRepository Users { get; }
    IStudentRepository Students { get; }
    ICompanyRepository Companies { get; }
    IUniversityRepository Universities { get; }
    IDepartmentRepository Departments { get; }
    ISkillRepository Skills { get; }
    IStudentSkillRepository StudentSkills { get; }
    IProjectRepository Projects { get; }
    IProjectModuleRepository ProjectModules { get; }
    IProjectRequiredSkillRepository ProjectRequiredSkills { get; }
    IApplicationRepository Applications { get; }
    IApplicationModuleProgressRepository ApplicationModuleProgresses { get; }
    IProjectGroupRepository ProjectGroups { get; }
    IGroupMemberRepository GroupMembers { get; }
    IConversationRepository Conversations { get; }
    IConversationParticipantRepository ConversationParticipants { get; }
    IMessageRepository Messages { get; }
    ICertificateRepository Certificates { get; }
    INotificationRepository Notifications { get; }
    IActivityLogRepository ActivityLogs { get; }
    IDashboardMetricRepository DashboardMetrics { get; }
    IUserSettingsRepository UserSettings { get; }
    IPaymentRepository Payments { get; }
    ICompletedOpportunityRepository CompletedOpportunities { get; }
    ICompanyReviewRepository CompanyReviews { get; }
    IStudentReviewRepository StudentReviews { get; }
    
    // Transaction management
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}

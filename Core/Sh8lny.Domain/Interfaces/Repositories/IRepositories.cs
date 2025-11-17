using Sh8lny.Domain.Entities;

namespace Sh8lny.Domain.Interfaces.Repositories;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetUserWithStudentProfileAsync(int userId, CancellationToken cancellationToken = default);
    Task<User?> GetUserWithCompanyProfileAsync(int userId, CancellationToken cancellationToken = default);
}

public interface IStudentRepository : IGenericRepository<Student>
{
    Task<Student?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<Student?> GetWithSkillsAsync(int studentId, CancellationToken cancellationToken = default);
    Task<Student?> GetWithApplicationsAsync(int studentId, CancellationToken cancellationToken = default);
    Task<Student?> GetWithReviewsAsync(int studentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Student>> GetByUniversityAsync(int universityId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Student>> GetBySkillAsync(int skillId, CancellationToken cancellationToken = default);
    Task UpdateRatingAsync(int studentId, CancellationToken cancellationToken = default);
}

public interface ICompanyRepository : IGenericRepository<Company>
{
    Task<Company?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<Company?> GetWithProjectsAsync(int companyId, CancellationToken cancellationToken = default);
    Task<Company?> GetWithReviewsAsync(int companyId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Company>> GetByIndustryAsync(string industry, CancellationToken cancellationToken = default);
    Task UpdateRatingAsync(int companyId, CancellationToken cancellationToken = default);
}

public interface IUniversityRepository : IGenericRepository<University>
{
    Task<University?> GetWithDepartmentsAsync(int universityId, CancellationToken cancellationToken = default);
    Task<IEnumerable<University>> GetActiveUniversitiesAsync(CancellationToken cancellationToken = default);
}

public interface IDepartmentRepository : IGenericRepository<Department>
{
    Task<IEnumerable<Department>> GetByUniversityAsync(int universityId, CancellationToken cancellationToken = default);
}

public interface ISkillRepository : IGenericRepository<Skill>
{
    Task<IEnumerable<Skill>> GetActiveSkillsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Skill>> GetSkillsByCategoryAsync(SkillCategory category, CancellationToken cancellationToken = default);
    Task<Skill?> GetByNameAsync(string skillName, CancellationToken cancellationToken = default);
}

public interface IStudentSkillRepository : IGenericRepository<StudentSkill>
{
    Task<IEnumerable<StudentSkill>> GetByStudentIdAsync(int studentId, CancellationToken cancellationToken = default);
    Task<bool> StudentHasSkillAsync(int studentId, int skillId, CancellationToken cancellationToken = default);
}

public interface IProjectRepository : IGenericRepository<Project>
{
    Task<Project?> GetWithRequiredSkillsAsync(int projectId, CancellationToken cancellationToken = default);
    Task<Project?> GetWithApplicationsAsync(int projectId, CancellationToken cancellationToken = default);
    Task<Project?> GetWithCompanyAsync(int projectId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Project>> GetByCompanyIdAsync(int companyId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Project>> GetActiveProjectsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Project>> SearchProjectsAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<IEnumerable<Project>> GetByStatusAsync(ProjectStatus status, CancellationToken cancellationToken = default);
    Task IncrementViewCountAsync(int projectId, CancellationToken cancellationToken = default);
}

public interface IProjectModuleRepository : IGenericRepository<ProjectModule>
{
    Task<IEnumerable<ProjectModule>> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken = default);
}

public interface IProjectRequiredSkillRepository : IGenericRepository<ProjectRequiredSkill>
{
    Task<IEnumerable<ProjectRequiredSkill>> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken = default);
}

public interface IApplicationModuleProgressRepository : IGenericRepository<ApplicationModuleProgress>
{
    Task<IEnumerable<ApplicationModuleProgress>> GetByApplicationIdAsync(int applicationId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ApplicationModuleProgress>> GetByProjectModuleIdAsync(int projectModuleId, CancellationToken cancellationToken = default);
    Task<ApplicationModuleProgress?> GetByApplicationAndModuleAsync(int applicationId, int projectModuleId, CancellationToken cancellationToken = default);
}

public interface IApplicationRepository : IGenericRepository<Application>
{
    Task<Application?> GetWithProjectAndStudentAsync(int applicationId, CancellationToken cancellationToken = default);
    Task<Application?> GetWithProgressAsync(int applicationId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Application>> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Application>> GetByStudentIdAsync(int studentId, CancellationToken cancellationToken = default);
    Task<Application?> GetByProjectAndStudentAsync(int projectId, int studentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Application>> GetByStatusAsync(ApplicationStatus status, CancellationToken cancellationToken = default);
}

public interface IProjectGroupRepository : IGenericRepository<ProjectGroup>
{
    Task<ProjectGroup?> GetWithMembersAsync(int groupId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProjectGroup>> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken = default);
}

public interface IGroupMemberRepository : IGenericRepository<GroupMember>
{
    Task<IEnumerable<GroupMember>> GetByGroupIdAsync(int groupId, CancellationToken cancellationToken = default);
    Task<IEnumerable<GroupMember>> GetByStudentIdAsync(int studentId, CancellationToken cancellationToken = default);
    Task<bool> IsStudentInGroupAsync(int groupId, int studentId, CancellationToken cancellationToken = default);
}

public interface IConversationRepository : IGenericRepository<Conversation>
{
    Task<Conversation?> GetWithParticipantsAsync(int conversationId, CancellationToken cancellationToken = default);
    Task<Conversation?> GetWithMessagesAsync(int conversationId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Conversation>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
}

public interface IConversationParticipantRepository : IGenericRepository<ConversationParticipant>
{
    Task<IEnumerable<ConversationParticipant>> GetByConversationIdAsync(int conversationId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ConversationParticipant>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<bool> IsUserParticipantAsync(int conversationId, int userId, CancellationToken cancellationToken = default);
}

public interface IMessageRepository : IGenericRepository<Message>
{
    Task<IEnumerable<Message>> GetByConversationIdAsync(int conversationId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Message>> GetByConversationIdAsync(int conversationId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<IEnumerable<Message>> GetUnreadMessagesAsync(int userId, CancellationToken cancellationToken = default);
    Task MarkAsReadAsync(int messageId, CancellationToken cancellationToken = default);
}

public interface ICertificateRepository : IGenericRepository<Certificate>
{
    Task<IEnumerable<Certificate>> GetByStudentIdAsync(int studentId, CancellationToken cancellationToken = default);
    Task<Certificate?> GetByCertificateNumberAsync(string certificateNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<Certificate>> GetByCompanyIdAsync(int companyId, CancellationToken cancellationToken = default);
}

public interface INotificationRepository : IGenericRepository<Notification>
{
    Task<IEnumerable<Notification>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Notification>> GetUnreadByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task MarkAsReadAsync(int notificationId, CancellationToken cancellationToken = default);
    Task MarkAllAsReadAsync(int userId, CancellationToken cancellationToken = default);
}

public interface IActivityLogRepository : IGenericRepository<ActivityLog>
{
    Task<IEnumerable<ActivityLog>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ActivityLog>> GetByActivityTypeAsync(string activityType, CancellationToken cancellationToken = default);
    Task<IEnumerable<ActivityLog>> GetRecentActivitiesAsync(int count, CancellationToken cancellationToken = default);
}

public interface IDashboardMetricRepository : IGenericRepository<DashboardMetric>
{
    Task<DashboardMetric?> GetLatestMetricAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<DashboardMetric>> GetMetricsByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<DashboardMetric?> GetByCompanyIdAsync(int companyId, CancellationToken cancellationToken = default);
}

public interface IUserSettingsRepository : IGenericRepository<UserSettings>
{
    Task<UserSettings?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
}

public interface IPaymentRepository : IGenericRepository<Payment>
{
    Task<IEnumerable<Payment>> GetByStudentIdAsync(int studentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Payment>> GetByCompanyIdAsync(int companyId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Payment>> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken = default);
    Task<Payment?> GetByTransactionIdAsync(string transactionId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Payment>> GetByStatusAsync(PaymentStatus status, CancellationToken cancellationToken = default);
}

public interface ICompletedOpportunityRepository : IGenericRepository<CompletedOpportunity>
{
    Task<IEnumerable<CompletedOpportunity>> GetByStudentIdAsync(int studentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<CompletedOpportunity>> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken = default);
    Task<CompletedOpportunity?> GetByApplicationIdAsync(int applicationId, CancellationToken cancellationToken = default);
    Task<IEnumerable<CompletedOpportunity>> GetVerifiedOpportunitiesAsync(CancellationToken cancellationToken = default);
}

public interface ICompanyReviewRepository : IGenericRepository<CompanyReview>
{
    Task<IEnumerable<CompanyReview>> GetByCompanyIdAsync(int companyId, CancellationToken cancellationToken = default);
    Task<IEnumerable<CompanyReview>> GetByStudentIdAsync(int studentId, CancellationToken cancellationToken = default);
    Task<CompanyReview?> GetByCompletedOpportunityIdAsync(int completedOpportunityId, CancellationToken cancellationToken = default);
    Task<IEnumerable<CompanyReview>> GetApprovedReviewsAsync(int companyId, CancellationToken cancellationToken = default);
}

public interface IStudentReviewRepository : IGenericRepository<StudentReview>
{
    Task<IEnumerable<StudentReview>> GetByStudentIdAsync(int studentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<StudentReview>> GetByCompanyIdAsync(int companyId, CancellationToken cancellationToken = default);
    Task<StudentReview?> GetByCompletedOpportunityIdAsync(int completedOpportunityId, CancellationToken cancellationToken = default);
    Task<IEnumerable<StudentReview>> GetApprovedReviewsAsync(int studentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<StudentReview>> GetPublicReviewsAsync(int studentId, CancellationToken cancellationToken = default);
}

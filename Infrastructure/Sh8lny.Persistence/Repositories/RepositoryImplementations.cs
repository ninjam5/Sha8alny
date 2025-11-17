using Microsoft.EntityFrameworkCore;
using Sh8lny.Domain.Entities;
using Sh8lny.Domain.Interfaces.Repositories;
using Sh8lny.Persistence.Contexts;

namespace Sh8lny.Persistence.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(Sha8lnyDbContext context) : base(context) { }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<User?> GetUserWithStudentProfileAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(u => u.Student)
                .ThenInclude(s => s!.University)
            .Include(u => u.Student)
                .ThenInclude(s => s!.Department)
            .FirstOrDefaultAsync(u => u.UserID == userId, cancellationToken);
    }

    public async Task<User?> GetUserWithCompanyProfileAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(u => u.Company)
            .FirstOrDefaultAsync(u => u.UserID == userId, cancellationToken);
    }
}

public class StudentRepository : GenericRepository<Student>, IStudentRepository
{
    public StudentRepository(Sha8lnyDbContext context) : base(context) { }

    public async Task<Student?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.UserID == userId, cancellationToken);
    }

    public async Task<Student?> GetWithSkillsAsync(int studentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.StudentSkills)
                .ThenInclude(ss => ss.Skill)
            .FirstOrDefaultAsync(s => s.StudentID == studentId, cancellationToken);
    }

    public async Task<Student?> GetWithApplicationsAsync(int studentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.Applications)
                .ThenInclude(a => a.Project)
            .FirstOrDefaultAsync(s => s.StudentID == studentId, cancellationToken);
    }

    public async Task<Student?> GetWithReviewsAsync(int studentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.ReceivedReviews)
                .ThenInclude(r => r.Company)
            .FirstOrDefaultAsync(s => s.StudentID == studentId, cancellationToken);
    }

    public async Task<IEnumerable<Student>> GetByUniversityAsync(int universityId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.UniversityID == universityId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Student>> GetBySkillAsync(int skillId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.StudentSkills.Any(ss => ss.SkillID == skillId))
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateRatingAsync(int studentId, CancellationToken cancellationToken = default)
    {
        var student = await _dbSet
            .Include(s => s.ReceivedReviews)
            .FirstOrDefaultAsync(s => s.StudentID == studentId, cancellationToken);

        if (student != null)
        {
            var approvedReviews = student.ReceivedReviews
                .Where(r => r.Status == ReviewStatus.Approved)
                .ToList();

            student.TotalReviews = approvedReviews.Count();
            student.AverageRating = approvedReviews.Any() 
                ? approvedReviews.Average(r => r.Rating) 
                : 0;
        }
    }
}

public class CompanyRepository : GenericRepository<Company>, ICompanyRepository
{
    public CompanyRepository(Sha8lnyDbContext context) : base(context) { }

    public async Task<Company?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.UserID == userId, cancellationToken);
    }

    public async Task<Company?> GetWithProjectsAsync(int companyId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Projects)
            .FirstOrDefaultAsync(c => c.CompanyID == companyId, cancellationToken);
    }

    public async Task<Company?> GetWithReviewsAsync(int companyId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Reviews)
                .ThenInclude(r => r.Student)
            .FirstOrDefaultAsync(c => c.CompanyID == companyId, cancellationToken);
    }

    public async Task<IEnumerable<Company>> GetByIndustryAsync(string industry, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.Industry == industry)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateRatingAsync(int companyId, CancellationToken cancellationToken = default)
    {
        var company = await _dbSet
            .Include(c => c.Reviews)
            .FirstOrDefaultAsync(c => c.CompanyID == companyId, cancellationToken);

        if (company != null)
        {
            var approvedReviews = company.Reviews
                .Where(r => r.Status == ReviewStatus.Approved)
                .ToList();

            company.TotalReviews = approvedReviews.Count();
            company.AverageRating = approvedReviews.Any() 
                ? approvedReviews.Average(r => r.Rating) 
                : 0;
        }
    }
}

public class ProjectRepository : GenericRepository<Project>, IProjectRepository
{
    public ProjectRepository(Sha8lnyDbContext context) : base(context) { }

    public async Task<Project?> GetWithRequiredSkillsAsync(int projectId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.ProjectRequiredSkills)
                .ThenInclude(rs => rs.Skill)
            .FirstOrDefaultAsync(p => p.ProjectID == projectId, cancellationToken);
    }

    public async Task<Project?> GetWithApplicationsAsync(int projectId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Applications)
                .ThenInclude(a => a.Student)
            .FirstOrDefaultAsync(p => p.ProjectID == projectId, cancellationToken);
    }

    public async Task<Project?> GetWithCompanyAsync(int projectId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Company)
            .FirstOrDefaultAsync(p => p.ProjectID == projectId, cancellationToken);
    }

    public async Task<IEnumerable<Project>> GetByCompanyIdAsync(int companyId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.CompanyID == companyId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Project>> GetActiveProjectsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.Status == ProjectStatus.Active && p.IsVisible)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Project>> SearchProjectsAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.ProjectName.Contains(searchTerm) || 
                        p.Description.Contains(searchTerm))
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Project>> GetByStatusAsync(ProjectStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.Status == status)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task IncrementViewCountAsync(int projectId, CancellationToken cancellationToken = default)
    {
        var project = await _dbSet.FindAsync(new object[] { projectId }, cancellationToken);
        if (project != null)
        {
            project.ViewCount++;
        }
    }
}

public class ProjectModuleRepository : GenericRepository<ProjectModule>, IProjectModuleRepository
{
    public ProjectModuleRepository(Sha8lnyDbContext context) : base(context) { }

    public async Task<IEnumerable<ProjectModule>> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(pm => pm.ProjectId == projectId)
            .OrderBy(pm => pm.OrderIndex)
            .ToListAsync(cancellationToken);
    }
}

// Create stub implementations for remaining repositories
public class UniversityRepository : GenericRepository<University>, IUniversityRepository
{
    public UniversityRepository(Sha8lnyDbContext context) : base(context) { }

    public async Task<University?> GetWithDepartmentsAsync(int universityId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(u => u.Departments)
            .FirstOrDefaultAsync(u => u.UniversityID == universityId, cancellationToken);
    }

    public async Task<IEnumerable<University>> GetActiveUniversitiesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(u => u.IsActive)
            .OrderBy(u => u.UniversityName)
            .ToListAsync(cancellationToken);
    }
}

public class DepartmentRepository : GenericRepository<Department>, IDepartmentRepository
{
    public DepartmentRepository(Sha8lnyDbContext context) : base(context) { }

    public async Task<IEnumerable<Department>> GetByUniversityAsync(int universityId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(d => d.UniversityID == universityId && d.IsActive)
            .ToListAsync(cancellationToken);
    }
}

public class SkillRepository : GenericRepository<Skill>, ISkillRepository
{
    public SkillRepository(Sha8lnyDbContext context) : base(context) { }

    public async Task<IEnumerable<Skill>> GetActiveSkillsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.IsActive)
            .OrderBy(s => s.SkillName)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Skill>> GetSkillsByCategoryAsync(SkillCategory category, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.SkillCategory == category && s.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<Skill?> GetByNameAsync(string skillName, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(s => s.SkillName == skillName, cancellationToken);
    }
}

public class StudentSkillRepository : GenericRepository<StudentSkill>, IStudentSkillRepository
{
    public StudentSkillRepository(Sha8lnyDbContext context) : base(context) { }

    public async Task<IEnumerable<StudentSkill>> GetByStudentIdAsync(int studentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(ss => ss.Skill)
            .Where(ss => ss.StudentID == studentId)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> StudentHasSkillAsync(int studentId, int skillId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(ss => ss.StudentID == studentId && ss.SkillID == skillId, cancellationToken);
    }
}

public class ApplicationRepository : GenericRepository<Application>, IApplicationRepository
{
    public ApplicationRepository(Sha8lnyDbContext context) : base(context) { }

    public async Task<Application?> GetWithProjectAndStudentAsync(int applicationId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.Project)
            .Include(a => a.Student)
            .FirstOrDefaultAsync(a => a.ApplicationID == applicationId, cancellationToken);
    }

    public async Task<Application?> GetWithProgressAsync(int applicationId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.Project)
                .ThenInclude(p => p.Modules)
            .Include(a => a.ModuleProgress)
            .FirstOrDefaultAsync(a => a.ApplicationID == applicationId, cancellationToken);
    }

    public async Task<IEnumerable<Application>> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.Student)
            .Where(a => a.ProjectID == projectId)
            .OrderByDescending(a => a.AppliedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Application>> GetByStudentIdAsync(int studentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.Project)
                .ThenInclude(p => p.Company)
            .Where(a => a.StudentID == studentId)
            .OrderByDescending(a => a.AppliedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Application?> GetByProjectAndStudentAsync(int projectId, int studentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(a => a.ProjectID == projectId && a.StudentID == studentId, cancellationToken);
    }

    public async Task<IEnumerable<Application>> GetByStatusAsync(ApplicationStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.Project)
            .Include(a => a.Student)
            .Where(a => a.Status == status)
            .ToListAsync(cancellationToken);
    }
}

public class ApplicationModuleProgressRepository : GenericRepository<ApplicationModuleProgress>, IApplicationModuleProgressRepository
{
    public ApplicationModuleProgressRepository(Sha8lnyDbContext context) : base(context) { }

    public async Task<IEnumerable<ApplicationModuleProgress>> GetByApplicationIdAsync(int applicationId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(mp => mp.ProjectModule)
            .Where(mp => mp.ApplicationId == applicationId)
            .OrderBy(mp => mp.ProjectModule.OrderIndex)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ApplicationModuleProgress>> GetByProjectModuleIdAsync(int projectModuleId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(mp => mp.Application)
            .Where(mp => mp.ProjectModuleId == projectModuleId)
            .ToListAsync(cancellationToken);
    }

    public async Task<ApplicationModuleProgress?> GetByApplicationAndModuleAsync(int applicationId, int projectModuleId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(mp => mp.ApplicationId == applicationId && mp.ProjectModuleId == projectModuleId, cancellationToken);
    }
}

// Stub implementations for remaining repositories
public class ProjectRequiredSkillRepository : GenericRepository<ProjectRequiredSkill>, IProjectRequiredSkillRepository
{
    public ProjectRequiredSkillRepository(Sha8lnyDbContext context) : base(context) { }

    public async Task<IEnumerable<ProjectRequiredSkill>> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(prs => prs.Skill)
            .Where(prs => prs.ProjectID == projectId)
            .ToListAsync(cancellationToken);
    }
}

public class ProjectGroupRepository : GenericRepository<ProjectGroup>, IProjectGroupRepository
{
    public ProjectGroupRepository(Sha8lnyDbContext context) : base(context) { }

    public async Task<ProjectGroup?> GetWithMembersAsync(int groupId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(pg => pg.GroupMembers)
                .ThenInclude(m => m.Student)
            .FirstOrDefaultAsync(pg => pg.GroupID == groupId, cancellationToken);
    }

    public async Task<IEnumerable<ProjectGroup>> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(pg => pg.ProjectID == projectId)
            .ToListAsync(cancellationToken);
    }
}

public class GroupMemberRepository : GenericRepository<GroupMember>, IGroupMemberRepository
{
    public GroupMemberRepository(Sha8lnyDbContext context) : base(context) { }

    public async Task<IEnumerable<GroupMember>> GetByGroupIdAsync(int groupId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(gm => gm.Student)
            .Where(gm => gm.GroupID == groupId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<GroupMember>> GetByStudentIdAsync(int studentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(gm => gm.ProjectGroup)
                .ThenInclude(g => g.Project)
            .Where(gm => gm.StudentID == studentId)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsStudentInGroupAsync(int groupId, int studentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(gm => gm.GroupID == groupId && gm.StudentID == studentId, cancellationToken);
    }
}

public class ConversationRepository : GenericRepository<Conversation>, IConversationRepository
{
    public ConversationRepository(Sha8lnyDbContext context) : base(context) { }

    public async Task<Conversation?> GetWithParticipantsAsync(int conversationId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Participants)
                .ThenInclude(p => p.User)
            .FirstOrDefaultAsync(c => c.ConversationID == conversationId, cancellationToken);
    }

    public async Task<Conversation?> GetWithMessagesAsync(int conversationId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Messages)
                .ThenInclude(m => m.Sender)
            .FirstOrDefaultAsync(c => c.ConversationID == conversationId, cancellationToken);
    }

    public async Task<IEnumerable<Conversation>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Participants)
            .Where(c => c.Participants.Any(p => p.UserID == userId))
            .OrderByDescending(c => c.LastMessageAt)
            .ToListAsync(cancellationToken);
    }
}

public class ConversationParticipantRepository : GenericRepository<ConversationParticipant>, IConversationParticipantRepository
{
    public ConversationParticipantRepository(Sha8lnyDbContext context) : base(context) { }

    public async Task<IEnumerable<ConversationParticipant>> GetByConversationIdAsync(int conversationId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(cp => cp.User)
            .Where(cp => cp.ConversationID == conversationId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ConversationParticipant>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(cp => cp.Conversation)
            .Where(cp => cp.UserID == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsUserParticipantAsync(int conversationId, int userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(cp => cp.ConversationID == conversationId && cp.UserID == userId, cancellationToken);
    }
}

public class MessageRepository : GenericRepository<Message>, IMessageRepository
{
    public MessageRepository(Sha8lnyDbContext context) : base(context) { }

    public async Task<IEnumerable<Message>> GetByConversationIdAsync(int conversationId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(m => m.Sender)
            .Where(m => m.ConversationID == conversationId)
            .OrderBy(m => m.SentAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Message>> GetByConversationIdAsync(int conversationId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(m => m.Sender)
            .Where(m => m.ConversationID == conversationId)
            .OrderByDescending(m => m.SentAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Message>> GetUnreadMessagesAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(m => m.Conversation)
                .ThenInclude(c => c.Participants)
            .Where(m => m.Conversation.Participants.Any(p => p.UserID == userId) &&
                        m.SenderID != userId &&
                        !m.IsRead)
            .ToListAsync(cancellationToken);
    }

    public async Task MarkAsReadAsync(int messageId, CancellationToken cancellationToken = default)
    {
        var message = await _dbSet.FindAsync(new object[] { messageId }, cancellationToken);
        if (message != null)
        {
            message.IsRead = true;
        }
    }
}

public class CertificateRepository : GenericRepository<Certificate>, ICertificateRepository
{
    public CertificateRepository(Sha8lnyDbContext context) : base(context) { }

    public async Task<IEnumerable<Certificate>> GetByStudentIdAsync(int studentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Company)
            .Include(c => c.Project)
            .Where(c => c.StudentID == studentId)
            .OrderByDescending(c => c.IssuedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Certificate?> GetByCertificateNumberAsync(string certificateNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Student)
            .Include(c => c.Company)
            .Include(c => c.Project)
            .FirstOrDefaultAsync(c => c.CertificateNumber == certificateNumber, cancellationToken);
    }

    public async Task<IEnumerable<Certificate>> GetByCompanyIdAsync(int companyId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Student)
            .Include(c => c.Project)
            .Where(c => c.CompanyID == companyId)
            .ToListAsync(cancellationToken);
    }
}

public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
{
    public NotificationRepository(Sha8lnyDbContext context) : base(context) { }

    public async Task<IEnumerable<Notification>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(n => n.UserID == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Notification>> GetUnreadByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(n => n.UserID == userId && !n.IsRead)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task MarkAsReadAsync(int notificationId, CancellationToken cancellationToken = default)
    {
        var notification = await _dbSet.FindAsync(new object[] { notificationId }, cancellationToken);
        if (notification != null)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
        }
    }

    public async Task MarkAllAsReadAsync(int userId, CancellationToken cancellationToken = default)
    {
        var notifications = await _dbSet
            .Where(n => n.UserID == userId && !n.IsRead)
            .ToListAsync(cancellationToken);

        foreach (var notification in notifications)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
        }
    }
}

public class ActivityLogRepository : GenericRepository<ActivityLog>, IActivityLogRepository
{
    public ActivityLogRepository(Sha8lnyDbContext context) : base(context) { }

    public async Task<IEnumerable<ActivityLog>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(al => al.UserID == userId)
            .OrderByDescending(al => al.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ActivityLog>> GetByActivityTypeAsync(string activityType, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(al => al.ActivityType == activityType)
            .OrderByDescending(al => al.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ActivityLog>> GetRecentActivitiesAsync(int count, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(al => al.User)
            .OrderByDescending(al => al.CreatedAt)
            .Take(count)
            .ToListAsync(cancellationToken);
    }
}

public class DashboardMetricRepository : GenericRepository<DashboardMetric>, IDashboardMetricRepository
{
    public DashboardMetricRepository(Sha8lnyDbContext context) : base(context) { }

    public async Task<DashboardMetric?> GetLatestMetricAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .OrderByDescending(dm => dm.MetricDate)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<DashboardMetric>> GetMetricsByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(dm => dm.MetricDate >= startDate && 
                         dm.MetricDate <= endDate)
            .OrderBy(dm => dm.MetricDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<DashboardMetric?> GetByCompanyIdAsync(int companyId, CancellationToken cancellationToken = default)
    {
        // Note: CompanyID property is commented out in DashboardMetric entity
        // Returning latest metric for now - this method needs entity update
        return await _dbSet
            .OrderByDescending(dm => dm.MetricDate)
            .FirstOrDefaultAsync(cancellationToken);
    }
}

public class UserSettingsRepository : GenericRepository<UserSettings>, IUserSettingsRepository
{
    public UserSettingsRepository(Sha8lnyDbContext context) : base(context) { }

    public async Task<UserSettings?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(us => us.UserID == userId, cancellationToken);
    }
}

public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
{
    public PaymentRepository(Sha8lnyDbContext context) : base(context) { }

    public async Task<IEnumerable<Payment>> GetByStudentIdAsync(int studentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Project)
            .Where(p => p.StudentID == studentId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Payment>> GetByCompanyIdAsync(int companyId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Project)
            .Include(p => p.Student)
            .Where(p => p.CompanyID == companyId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Payment>> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Student)
            .Where(p => p.ProjectID == projectId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Payment?> GetByTransactionIdAsync(string transactionId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(p => p.TransactionID == transactionId, cancellationToken);
    }

    public async Task<IEnumerable<Payment>> GetByStatusAsync(PaymentStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.Status == status)
            .ToListAsync(cancellationToken);
    }
}

public class CompletedOpportunityRepository : GenericRepository<CompletedOpportunity>, ICompletedOpportunityRepository
{
    public CompletedOpportunityRepository(Sha8lnyDbContext context) : base(context) { }

    public async Task<IEnumerable<CompletedOpportunity>> GetByStudentIdAsync(int studentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(co => co.Project)
            .Where(co => co.StudentID == studentId)
            .OrderByDescending(co => co.CompletedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<CompletedOpportunity>> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(co => co.Student)
            .Where(co => co.ProjectID == projectId)
            .ToListAsync(cancellationToken);
    }

    public async Task<CompletedOpportunity?> GetByApplicationIdAsync(int applicationId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(co => co.ApplicationID == applicationId, cancellationToken);
    }

    public async Task<IEnumerable<CompletedOpportunity>> GetVerifiedOpportunitiesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(co => co.Student)
            .Include(co => co.Project)
            .Where(co => co.IsVerified)
            .ToListAsync(cancellationToken);
    }
}

public class CompanyReviewRepository : GenericRepository<CompanyReview>, ICompanyReviewRepository
{
    public CompanyReviewRepository(Sha8lnyDbContext context) : base(context) { }

    public async Task<IEnumerable<CompanyReview>> GetByCompanyIdAsync(int companyId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(cr => cr.Student)
            .Where(cr => cr.CompanyID == companyId)
            .OrderByDescending(cr => cr.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<CompanyReview>> GetByStudentIdAsync(int studentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(cr => cr.Company)
            .Where(cr => cr.StudentID == studentId)
            .OrderByDescending(cr => cr.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<CompanyReview?> GetByCompletedOpportunityIdAsync(int completedOpportunityId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(cr => cr.CompletedOpportunityID == completedOpportunityId, cancellationToken);
    }

    public async Task<IEnumerable<CompanyReview>> GetApprovedReviewsAsync(int companyId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(cr => cr.CompanyID == companyId && cr.Status == ReviewStatus.Approved)
            .ToListAsync(cancellationToken);
    }
}

public class StudentReviewRepository : GenericRepository<StudentReview>, IStudentReviewRepository
{
    public StudentReviewRepository(Sha8lnyDbContext context) : base(context) { }

    public async Task<IEnumerable<StudentReview>> GetByStudentIdAsync(int studentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(sr => sr.Company)
            .Where(sr => sr.StudentID == studentId)
            .OrderByDescending(sr => sr.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<StudentReview>> GetByCompanyIdAsync(int companyId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(sr => sr.Student)
            .Where(sr => sr.CompanyID == companyId)
            .OrderByDescending(sr => sr.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<StudentReview?> GetByCompletedOpportunityIdAsync(int completedOpportunityId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(sr => sr.CompletedOpportunityID == completedOpportunityId, cancellationToken);
    }

    public async Task<IEnumerable<StudentReview>> GetApprovedReviewsAsync(int studentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(sr => sr.StudentID == studentId && sr.Status == ReviewStatus.Approved)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<StudentReview>> GetPublicReviewsAsync(int studentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(sr => sr.Company)
            .Where(sr => sr.StudentID == studentId && sr.IsPublic && sr.Status == ReviewStatus.Approved)
            .ToListAsync(cancellationToken);
    }
}

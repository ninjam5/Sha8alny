using Microsoft.EntityFrameworkCore.Storage;
using Sh8lny.Domain.Interfaces;
using Sh8lny.Domain.Interfaces.Repositories;
using Sh8lny.Persistence.Contexts;

namespace Sh8lny.Persistence.Repositories;

/// <summary>
/// Unit of Work implementation for managing transactions and repositories
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly Sha8lnyDbContext _context;
    private IDbContextTransaction? _transaction;

    // Repository lazy initialization fields
    private IUserRepository? _users;
    private IStudentRepository? _students;
    private ICompanyRepository? _companies;
    private IUniversityRepository? _universities;
    private IDepartmentRepository? _departments;
    private ISkillRepository? _skills;
    private IStudentSkillRepository? _studentSkills;
    private IProjectRepository? _projects;
    private IProjectModuleRepository? _projectModules;
    private IProjectRequiredSkillRepository? _projectRequiredSkills;
    private IApplicationRepository? _applications;
    private IApplicationModuleProgressRepository? _applicationModuleProgresses;
    private IProjectGroupRepository? _projectGroups;
    private IGroupMemberRepository? _groupMembers;
    private IConversationRepository? _conversations;
    private IConversationParticipantRepository? _conversationParticipants;
    private IMessageRepository? _messages;
    private ICertificateRepository? _certificates;
    private INotificationRepository? _notifications;
    private IActivityLogRepository? _activityLogs;
    private IDashboardMetricRepository? _dashboardMetrics;
    private IUserSettingsRepository? _userSettings;
    private IPaymentRepository? _payments;
    private ICompletedOpportunityRepository? _completedOpportunities;
    private ICompanyReviewRepository? _companyReviews;
    private IStudentReviewRepository? _studentReviews;

    public UnitOfWork(Sha8lnyDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    // Repository properties with lazy initialization
    public IUserRepository Users => _users ??= new UserRepository(_context);
    public IStudentRepository Students => _students ??= new StudentRepository(_context);
    public ICompanyRepository Companies => _companies ??= new CompanyRepository(_context);
    public IUniversityRepository Universities => _universities ??= new UniversityRepository(_context);
    public IDepartmentRepository Departments => _departments ??= new DepartmentRepository(_context);
    public ISkillRepository Skills => _skills ??= new SkillRepository(_context);
    public IStudentSkillRepository StudentSkills => _studentSkills ??= new StudentSkillRepository(_context);
    public IProjectRepository Projects => _projects ??= new ProjectRepository(_context);
    public IProjectModuleRepository ProjectModules => _projectModules ??= new ProjectModuleRepository(_context);
    public IProjectRequiredSkillRepository ProjectRequiredSkills => _projectRequiredSkills ??= new ProjectRequiredSkillRepository(_context);
    public IApplicationRepository Applications => _applications ??= new ApplicationRepository(_context);
    public IApplicationModuleProgressRepository ApplicationModuleProgresses => _applicationModuleProgresses ??= new ApplicationModuleProgressRepository(_context);
    public IProjectGroupRepository ProjectGroups => _projectGroups ??= new ProjectGroupRepository(_context);
    public IGroupMemberRepository GroupMembers => _groupMembers ??= new GroupMemberRepository(_context);
    public IConversationRepository Conversations => _conversations ??= new ConversationRepository(_context);
    public IConversationParticipantRepository ConversationParticipants => _conversationParticipants ??= new ConversationParticipantRepository(_context);
    public IMessageRepository Messages => _messages ??= new MessageRepository(_context);
    public ICertificateRepository Certificates => _certificates ??= new CertificateRepository(_context);
    public INotificationRepository Notifications => _notifications ??= new NotificationRepository(_context);
    public IActivityLogRepository ActivityLogs => _activityLogs ??= new ActivityLogRepository(_context);
    public IDashboardMetricRepository DashboardMetrics => _dashboardMetrics ??= new DashboardMetricRepository(_context);
    public IUserSettingsRepository UserSettings => _userSettings ??= new UserSettingsRepository(_context);
    public IPaymentRepository Payments => _payments ??= new PaymentRepository(_context);
    public ICompletedOpportunityRepository CompletedOpportunities => _completedOpportunities ??= new CompletedOpportunityRepository(_context);
    public ICompanyReviewRepository CompanyReviews => _companyReviews ??= new CompanyReviewRepository(_context);
    public IStudentReviewRepository StudentReviews => _studentReviews ??= new StudentReviewRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            
            if (_transaction != null)
            {
                await _transaction.CommitAsync(cancellationToken);
            }
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}

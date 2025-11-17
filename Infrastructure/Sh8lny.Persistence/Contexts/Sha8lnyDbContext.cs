using Microsoft.EntityFrameworkCore;
using Sh8lny.Persistence.Configurations;
using Sh8lny.Domain.Entities;

namespace Sh8lny.Persistence.Contexts
{
    /// <summary>
    /// Sha8lny Platform Database Context
    /// Student Training & Internship Management Platform
    /// </summary>
    public class Sha8lnyDbContext : DbContext
    {
        public Sha8lnyDbContext(DbContextOptions<Sha8lnyDbContext> options)
            : base(options)
        {
        }

        // Core Entities
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Student> Students { get; set; } = null!;
        public DbSet<Company> Companies { get; set; } = null!;
        public DbSet<University> Universities { get; set; } = null!;
        public DbSet<Department> Departments { get; set; } = null!;

        // Skills
        public DbSet<Skill> Skills { get; set; } = null!;
        public DbSet<StudentSkill> StudentSkills { get; set; } = null!;

        // Projects & Applications
        public DbSet<Project> Projects { get; set; } = null!;
        public DbSet<ProjectRequiredSkill> ProjectRequiredSkills { get; set; } = null!;
        public DbSet<ProjectModule> ProjectModules { get; set; } = null!;
        public DbSet<Application> Applications { get; set; } = null!;
        public DbSet<ApplicationModuleProgress> ApplicationModuleProgress { get; set; } = null!;

        // Teams & Collaboration
        public DbSet<ProjectGroup> ProjectGroups { get; set; } = null!;
        public DbSet<GroupMember> GroupMembers { get; set; } = null!;

        // Messaging
        public DbSet<Conversation> Conversations { get; set; } = null!;
        public DbSet<ConversationParticipant> ConversationParticipants { get; set; } = null!;
        public DbSet<Message> Messages { get; set; } = null!;

        // Analytics & Achievements
        public DbSet<Certificate> Certificates { get; set; } = null!;
        public DbSet<Notification> Notifications { get; set; } = null!;
        public DbSet<ActivityLog> ActivityLogs { get; set; } = null!;
        public DbSet<DashboardMetric> DashboardMetrics { get; set; } = null!;

        // Settings
        public DbSet<UserSettings> UserSettings { get; set; } = null!;

        // Payments & Completed Opportunities
        public DbSet<Payment> Payments { get; set; } = null!;
        public DbSet<CompletedOpportunity> CompletedOpportunities { get; set; } = null!;

        // Reviews
        public DbSet<CompanyReview> CompanyReviews { get; set; } = null!;
        public DbSet<StudentReview> StudentReviews { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply all configurations from separate files
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new StudentConfiguration());
            modelBuilder.ApplyConfiguration(new CompanyConfiguration());
            modelBuilder.ApplyConfiguration(new UniversityConfiguration());
            modelBuilder.ApplyConfiguration(new DepartmentConfiguration());
            modelBuilder.ApplyConfiguration(new SkillConfiguration());
            modelBuilder.ApplyConfiguration(new StudentSkillConfiguration());
            modelBuilder.ApplyConfiguration(new ProjectConfiguration());
            modelBuilder.ApplyConfiguration(new ProjectRequiredSkillConfiguration());
            modelBuilder.ApplyConfiguration(new ProjectModuleConfiguration());
            modelBuilder.ApplyConfiguration(new ApplicationConfiguration());
            modelBuilder.ApplyConfiguration(new ApplicationModuleProgressConfiguration());
            modelBuilder.ApplyConfiguration(new ProjectGroupConfiguration());
            modelBuilder.ApplyConfiguration(new GroupMemberConfiguration());
            modelBuilder.ApplyConfiguration(new ConversationConfiguration());
            modelBuilder.ApplyConfiguration(new ConversationParticipantConfiguration());
            modelBuilder.ApplyConfiguration(new MessageConfiguration());
            modelBuilder.ApplyConfiguration(new CertificateConfiguration());
            modelBuilder.ApplyConfiguration(new NotificationConfiguration());
            modelBuilder.ApplyConfiguration(new ActivityLogConfiguration());
            modelBuilder.ApplyConfiguration(new DashboardMetricConfiguration());
            modelBuilder.ApplyConfiguration(new UserSettingsConfiguration());
            modelBuilder.ApplyConfiguration(new PaymentConfiguration());
            modelBuilder.ApplyConfiguration(new CompletedOpportunityConfiguration());
            modelBuilder.ApplyConfiguration(new CompanyReviewConfiguration());
            modelBuilder.ApplyConfiguration(new StudentReviewConfiguration());

            // Check if using SQLite, and if so, override SQL Server-specific functions
            if (Database.ProviderName == "Microsoft.EntityFrameworkCore.Sqlite")
            {
                // Find all properties with GETDATE() and replace with CURRENT_TIMESTAMP
                foreach (var entityType in modelBuilder.Model.GetEntityTypes())
                {
                    foreach (var property in entityType.GetProperties())
                    {
                        var defaultValueSql = property.GetDefaultValueSql();
                        if (defaultValueSql == "GETDATE()")
                        {
                            property.SetDefaultValueSql("CURRENT_TIMESTAMP");
                        }
                    }
                }
            }

            // Global query filters (soft delete pattern if needed)
            // modelBuilder.Entity<User>().HasQueryFilter(u => u.IsActive);

            // Seed initial data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Skills (from Figma analysis)
            modelBuilder.Entity<Skill>().HasData(
                new Skill { SkillID = 1, SkillName = "Backend", SkillCategory = SkillCategory.Backend },
                new Skill { SkillID = 2, SkillName = "Frontend", SkillCategory = SkillCategory.Frontend },
                new Skill { SkillID = 3, SkillName = "UI&UX", SkillCategory = SkillCategory.UIUX },
                new Skill { SkillID = 4, SkillName = "Mobile App", SkillCategory = SkillCategory.Mobile },
                new Skill { SkillID = 5, SkillName = "Web Development", SkillCategory = SkillCategory.Frontend },
                new Skill { SkillID = 6, SkillName = "AI", SkillCategory = SkillCategory.AIML },
                new Skill { SkillID = 7, SkillName = "Big Data", SkillCategory = SkillCategory.Data },
                new Skill { SkillID = 8, SkillName = "Testing", SkillCategory = SkillCategory.Testing },
                new Skill { SkillID = 9, SkillName = "Marketing", SkillCategory = SkillCategory.Marketing },
                new Skill { SkillID = 10, SkillName = "Social Media", SkillCategory = SkillCategory.Marketing },
                new Skill { SkillID = 11, SkillName = "Photoshop", SkillCategory = SkillCategory.Design },
                new Skill { SkillID = 12, SkillName = "Cybersecurity", SkillCategory = SkillCategory.Security },
                new Skill { SkillID = 13, SkillName = "Flutter", SkillCategory = SkillCategory.Mobile }
            );
        }

        // Override SaveChanges to automatically update timestamps
        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                var entityType = entry.Entity.GetType();
                
                // Set CreatedAt for newly added entities
                if (entry.State == EntityState.Added)
                {
                    var createdAtProp = entityType.GetProperty("CreatedAt");
                    if (createdAtProp != null && createdAtProp.PropertyType == typeof(DateTime))
                    {
                        var currentValue = createdAtProp.GetValue(entry.Entity);
                        if (currentValue == null || (DateTime)currentValue == default)
                        {
                            entry.Property("CreatedAt").CurrentValue = DateTime.UtcNow;
                        }
                    }
                }
                
                // Update UpdatedAt for modified entities
                if (entry.State == EntityState.Modified)
                {
                    var updatedAtProp = entityType.GetProperty("UpdatedAt");
                    if (updatedAtProp != null && updatedAtProp.PropertyType == typeof(DateTime))
                    {
                        entry.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
                    }
                }
            }
        }
    }
}

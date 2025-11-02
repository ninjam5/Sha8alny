using Microsoft.EntityFrameworkCore;
using Sh8lny.Domain.Models;

namespace Sh8lny.Persistence.Contexts;

// Main EF Core database context
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // Apply all Fluent API configurations from assembly
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    // Core entities
    public DbSet<User> Users { get; set; }
    public DbSet<StudentProfile> StudentProfiles { get; set; }
    public DbSet<CompanyProfile> CompanyProfiles { get; set; }
    
    // Opportunity management
    public DbSet<Category> Categories { get; set; }
    public DbSet<Opportunity> Opportunities { get; set; }
    public DbSet<Application> Applications { get; set; }
    public DbSet<CompletedOpportunity> CompletedOpportunities { get; set; }
    
    // Transactions and feedback
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Review> Reviews { get; set; }
    
    // Communication
    public DbSet<Message> Messages { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    
    // Skills management
    public DbSet<Skill> Skills { get; set; }
    public DbSet<StudentSkill> StudentSkills { get; set; }
}

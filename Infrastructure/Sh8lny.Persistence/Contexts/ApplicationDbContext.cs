using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sh8lny.Domain.Models;

namespace Sh8lny.Persistence.Contexts
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>options):base(options)
        {
            
        }

        override protected void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<StudentProfile> StudentProfiles { get; set; }
        public DbSet<CompanyProfile> CompanyProfiles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Opportunity> Opportunities { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<CompletedOpportunity> CompletedOpportunities { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        public DbSet<Skill> Skills { get; set; }
        public DbSet<StudentSkill> StudentSkills { get; set; }
    }
}

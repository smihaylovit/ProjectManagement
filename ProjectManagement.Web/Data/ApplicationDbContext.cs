using Microsoft.EntityFrameworkCore;
using ProjectManagement.Web.Models.Entities;

namespace ProjectManagement.Web.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<TimeLog> TimeLogs { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<TimeLog>()
                .ToTable(tl => tl.HasCheckConstraint("CK_TimeLogs_Hours",
                    $"[Hours] >= {GlobalConstants.TimeLogMinHours} AND [Hours] <= {GlobalConstants.TimeLogMaxHours}"));
        }
    }
}

using Microsoft.EntityFrameworkCore;
using SmartFeedbackPortal.API.Models;

namespace SmartFeedbackPortal.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }

        // Optional: Customize model relationships
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User → Feedback relationship
            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.User)
                .WithMany(u => u.Feedbacks)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Optional: Add unique constraint on Username
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();
        }
    }
}

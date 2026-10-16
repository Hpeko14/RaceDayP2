using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Models;

namespace RaceDayAPI.Data
{
    public class RaceDayContext : DbContext
    {
        public RaceDayContext(DbContextOptions<RaceDayContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Event> Events => Set<Event>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Enrollment> Enrollments => Set<Enrollment>();
        public DbSet<Result> Results => Set<Result>();
        public DbSet<RaceRoute> Routes => Set<RaceRoute>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Role must be Organiser or Participant (same check as the Part 1 script)
            modelBuilder.Entity<User>()
                .ToTable(t => t.HasCheckConstraint("CK_Users_Role", "[Role] IN ('Organiser','Participant')"));

            // RegistrationDate defaults to today's date
            modelBuilder.Entity<Enrollment>()
                .Property(e => e.RegistrationDate)
                .HasDefaultValueSql("GETDATE()");

            // Foreign keys
            modelBuilder.Entity<Event>().HasOne<User>().WithMany().HasForeignKey(e => e.OrganiserId);
            modelBuilder.Entity<Category>().HasOne<Event>().WithMany().HasForeignKey(c => c.EventId);
            modelBuilder.Entity<Enrollment>().HasOne<User>().WithMany().HasForeignKey(e => e.UserId);
            modelBuilder.Entity<Enrollment>().HasOne<Category>().WithMany().HasForeignKey(e => e.CategoryId);
            modelBuilder.Entity<Result>().HasOne<Enrollment>().WithMany().HasForeignKey(r => r.EnrollmentId);
            modelBuilder.Entity<RaceRoute>().HasOne<Event>().WithMany().HasForeignKey(r => r.EventId);

            // No cascade deletes, as in the Part 1 script. SQL Server also rejects the
            // cascade paths this schema would otherwise create.
            foreach (var fk in modelBuilder.Model.GetEntityTypes().SelectMany(t => t.GetForeignKeys()))
            {
                fk.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}




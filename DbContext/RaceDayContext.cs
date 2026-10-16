using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Models;



namespace RaceDayAPI.Data
{//start of namespace
    public class RaceDayContext : DbContext
    {// start of class
        public RaceDayContext(DbContextOptions<RaceDayContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Result> Results { get; set; }

    }// end of class
}// end of namespace




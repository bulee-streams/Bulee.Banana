using API.Models;
using Microsoft.EntityFrameworkCore;


namespace API.Context
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options)
            :base(options) {}

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>().ToTable("User");
        }
    }
}

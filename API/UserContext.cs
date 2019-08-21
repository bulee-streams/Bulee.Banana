using System;
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
            builder.Entity<User>().ToTable("User").HasData(
            new User() { Id = Guid.NewGuid(), Username = "user",
                                              Email = "user@email.com",
                                              EmailConfirmationToken = Guid.Parse("A6A46A35-5165-4AB5-9E19-12764CFC2144"),
                                              EmailConfirmed = false });
        }
    }
}

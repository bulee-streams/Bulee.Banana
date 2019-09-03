using System;
using API.Models;
using Microsoft.EntityFrameworkCore;


namespace API.Helpers
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options)
            :base(options) {}

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            var encryptor = new PasswordEncryption();
            var password = encryptor.HashReturnSalt("password");

            builder.Entity<User>().ToTable("User").HasData
                (new User()
                {
                    Username = "user",
                    Email = "user@email.com",
                    EmailConfirmationToken = Guid.Parse("A6A46A35-5165-4AB5-9E19-12764CFC2144"),
                    EmailConfirmed = false,
                    Salt = password.Item1,
                    Password = password.Item2,
                    PassworResetToken = Guid.Parse("214065DD-36B2-4E5E-A67B-37AAB766BAFA")
                });
        }
    }
}

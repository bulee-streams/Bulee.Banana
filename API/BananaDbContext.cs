using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using API.Roles;
using API.Models;


namespace API.Context
{
    public class BananaDbContext : IdentityDbContext<User, UserRole, Guid>
    {
        public BananaDbContext(DbContextOptions<BananaDbContext> options)
            :base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}

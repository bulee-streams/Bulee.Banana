using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using API.Models;

namespace API.Context
{
    public class UserDbContext : DbContext, IUserDbContext<RegisterUser>
    {
        public UserDbContext(DbContextOptions<UserDbContext> options)
            :base(options) { }

        private DbSet<RegisterUser> users { get; set; }

        public Task Add(RegisterUser user) => base.AddAsync(user);

        public void Update(RegisterUser user) => base.Update(user);

        public void Remove(RegisterUser user) => base.Remove(user);

        public Task<bool> Exists(Guid id) => users.AnyAsync(u => u.Id == id);

        public Task<RegisterUser> Get(Guid id) => users.SingleOrDefaultAsync(u => u.Id == id);

        public Task<List<RegisterUser>> GetAll() => users
                                                   .OrderBy(t => t.TimeAdded)
                                                   .ToListAsync();

        public Task<int> SaveChangesAsync() => base.SaveChangesAsync();
    }
}

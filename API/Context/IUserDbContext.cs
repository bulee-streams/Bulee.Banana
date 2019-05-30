using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace API.Context
{
    public interface IUserDbContext<TEntity> where TEntity : class
    {
        Task Add(TEntity item);
        void Update(TEntity item);
        void Remove(TEntity item);
        Task<bool> Exists(Guid id);
        Task<TEntity> Get(Guid id);
        Task<List<TEntity>> GetAll();
        Task<int> SaveChangesAsync();
    }
}

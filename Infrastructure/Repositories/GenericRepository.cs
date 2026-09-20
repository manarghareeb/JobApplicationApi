using Application.Interfaces.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class GenericRepository<TEntity>(ApplicationDbContext _context) : IGenericRepository<TEntity> where TEntity : class
    {
        private readonly DbSet<TEntity> _dbSet = _context.Set<TEntity>();
        public async Task AddAsync(TEntity entity) => await _dbSet.AddAsync(entity);

        public void Delete(TEntity entity) => _dbSet.Remove(entity);

        public IQueryable<TEntity> GetAll() => _dbSet;

        public async Task<TEntity?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);

        public void Update(TEntity entity) => _dbSet.Update(entity);

    }
}

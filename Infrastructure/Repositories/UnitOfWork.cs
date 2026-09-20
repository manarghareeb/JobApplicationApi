using Application.Interfaces;
using Application.Interfaces.Repositories;
using Infrastructure.Persistence;
using System.Collections.Concurrent;

namespace Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        private ConcurrentDictionary<Type, object> _repositories = new();
        public IJobApplicationRepository JobApplicationRepository { get; }
        public UnitOfWork(ApplicationDbContext dbContext, IJobApplicationRepository jobApplicationRepository)
        {
            _dbContext = dbContext;
            JobApplicationRepository = jobApplicationRepository;
        }
        public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class
            => (IGenericRepository<TEntity>)_repositories.
            GetOrAdd(typeof(TEntity), (_) => new GenericRepository<TEntity>(_dbContext));

        public async Task<int> SaveChangesAsync() => await _dbContext.SaveChangesAsync();
    }
}

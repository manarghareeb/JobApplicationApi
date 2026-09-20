using Application.Interfaces.Repositories;

namespace Application.Interfaces
{
    public interface IUnitOfWork
    {
        IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class;
        IJobApplicationRepository JobApplicationRepository { get; }
        Task<int> SaveChangesAsync();
    }
}

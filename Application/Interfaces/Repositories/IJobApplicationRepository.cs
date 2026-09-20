using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface IJobApplicationRepository : IGenericRepository<JobApplication>
    {
        IQueryable<JobApplication> GetAllJobApplications(int jobId);
        Task<JobApplication?> GetByIdWithJobAsync(int applicationId);
    }
}

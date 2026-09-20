using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class JobApplicationRepository(ApplicationDbContext _context) : GenericRepository<JobApplication>(_context), IJobApplicationRepository
    {
        public async Task<JobApplication?> GetByIdWithJobAsync(int applicationId)
            => await _context.JobApplications.Include(a => a.Job).FirstOrDefaultAsync(a => a.Id == applicationId);
        IQueryable<JobApplication> IJobApplicationRepository.GetAllJobApplications(int jobId)
            => _context.JobApplications.Where(j => j.JobId == jobId);
    }
}

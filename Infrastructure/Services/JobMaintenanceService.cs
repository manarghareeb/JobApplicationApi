using Application.Interfaces;
using Application.Interfaces.Services;
using Domain.Entities;

namespace Infrastructure.Services
{
    public class JobMaintenanceService(IUnitOfWork _unitOfWork) : IJobMaintenanceService
    {
        public async Task CloseExpiredJobsAsync()
        {
            var jobs = _unitOfWork.GetRepository<Job>().GetAll()
                .Where(j => j.IsActive && j.CloseAt.HasValue && j.CloseAt.Value <= DateTime.UtcNow).ToList();
            foreach (var job in jobs)
            {
                job.IsActive = false;
                job.ClosedAt = DateTime.UtcNow;
                _unitOfWork.GetRepository<Job>().Update(job);
            }
            if (jobs.Count > 0)
            {
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}

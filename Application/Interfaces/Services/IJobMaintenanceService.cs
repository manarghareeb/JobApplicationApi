namespace Application.Interfaces.Services
{
    public interface IJobMaintenanceService
    {
        Task CloseExpiredJobsAsync();
    }
}

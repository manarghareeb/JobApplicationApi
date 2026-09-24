namespace Application.Interfaces.Services
{
    public interface INotificationService
    {
        void NotifyRecruiter(int applicationId);
        void NotifyCandidate(int applicationId);
    }
}

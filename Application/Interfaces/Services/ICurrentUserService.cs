namespace Application.Interfaces.Services
{
    public interface ICurrentUserService
    {
        Guid UserId { get; }
        int RecruiterId { get; }
        int CandidateId { get; }
        bool IsAuthenticated { get; }
    }
}
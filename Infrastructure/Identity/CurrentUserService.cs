using Application.Interfaces.Services;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Infrastructure.Identity
{
    public class CurrentUserService(IHttpContextAccessor httpContextAccessor, ApplicationDbContext dbContext) : ICurrentUserService
    {
        public bool IsAuthenticated => httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
        public Guid UserId
        {
            get
            {
                var userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrWhiteSpace(userId))
                    throw new UnauthorizedAccessException("User is not authenticated.");
                if (!Guid.TryParse(userId, out var id))
                    throw new UnauthorizedAccessException("Invalid user identifier.");
                return id;
            }
        }
        public int RecruiterId { 
            get { 
                var recruiter = dbContext.Recruiters.FirstOrDefault(r => r.UserId == UserId); 
                if (recruiter is null) 
                    throw new UnauthorizedAccessException("Current user is not a recruiter."); 
                return recruiter.Id; 
            }
        }
        public int CandidateId
        {
            get
            {
                var candidate = dbContext.Candidates.FirstOrDefault(c => c.UserId == UserId);
                if (candidate is null)
                    throw new UnauthorizedAccessException("Current user is not a candidate.");
                return candidate.Id;
            }
        }
    }
}
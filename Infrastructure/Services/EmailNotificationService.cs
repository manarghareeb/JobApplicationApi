using Application.Interfaces;
using Application.Interfaces.Services;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class EmailNotificationService(IUnitOfWork _unitOfWork, ILogger<EmailNotificationService> _logger) : INotificationService
    {
        public void NotifyCandidate(int applicationId)
        {
            var application = _unitOfWork.GetRepository<JobApplication>().GetAll().FirstOrDefault(a => a.Id == applicationId);
            if (application is null)
            {
                _logger.LogWarning("Application {ApplicationId} is not found.", applicationId);
                return;
            }
            _logger.LogInformation("Send Email: Candidate {CandidateId} has cancelled Application {ApplicationId}.", 
                application.CandidateId, applicationId);
        }

        public void NotifyRecruiter(int applicationId)
        {
            var application = _unitOfWork.GetRepository<JobApplication>().GetAll().FirstOrDefault(a => a.Id == applicationId);

            if (application is null)
            {
                _logger.LogWarning("application {applicationId}is not found ", applicationId);
                return;
            }
            _logger.LogInformation("Send Email :  cadidate {CandidateId} has applied to {JobId} and applicationId is {applicationId}",
                application.CandidateId, application.JobId, applicationId);
        }
    }
}

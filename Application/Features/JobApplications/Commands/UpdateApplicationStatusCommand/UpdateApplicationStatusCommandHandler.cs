using Application.DTOs.JobApplicationDto;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Features.JobApplications.Commands.UpdateApplicationStatusCommand
{
    internal class UpdateApplicationStatusCommandHandler(IUnitOfWork _unitOfWork, IMapper _mapper, ICurrentUserService _currentUserService
        , IBackgroundJobScheduler _backgroundJobScheduler) : IRequestHandler<UpdateApplicationStatusCommand, JobApplicationDto>
    {
        public async Task<JobApplicationDto> Handle(UpdateApplicationStatusCommand request, CancellationToken cancellationToken)
        {
            var application = await _unitOfWork.JobApplicationRepository.GetByIdWithJobAsync(request.ApplicationId);
            if (application is null)
                throw new NotFoundException("Application not found.");
            if (application.Job.RecruiterId != _currentUserService.RecruiterId)
                throw new ForbiddenException("You are not the owner of this job.");
            if (application.JobApplicationStatus == JobApplicationStatus.Cancelled)
                throw new BadRequestException("A cancelled application cannot be updated.");
            ValidateStatusTransition(application.JobApplicationStatus, request.UpdateJobApplicationStatusDto.Status);
            application.JobApplicationStatus = request.UpdateJobApplicationStatusDto.Status;
            application.StatusUpdatedAt = DateTime.UtcNow;
            _unitOfWork.GetRepository<JobApplication>().Update(application);
            await _unitOfWork.SaveChangesAsync();
            _backgroundJobScheduler.Schedule<INotificationService>(s => s.NotifyRecruiter(application.Id), TimeSpan.FromMinutes(1));
            return _mapper.Map<JobApplicationDto>(application);
        }
        private void ValidateStatusTransition(JobApplicationStatus currentStatus, JobApplicationStatus newStatus)
        {
            if (currentStatus == newStatus)
                throw new BadRequestException("The application already has this status.");
            if (currentStatus == JobApplicationStatus.Cancelled)
                throw new BadRequestException("A cancelled application cannot be reopened.");
            var allowedTransitions = new Dictionary<JobApplicationStatus, JobApplicationStatus[]>
                {
                    {
                        JobApplicationStatus.Applied, new[]
                        {
                            JobApplicationStatus.UnderReview,
                            JobApplicationStatus.Rejected
                        }
                    },
                    {
                        JobApplicationStatus.UnderReview, new[]
                        {
                            JobApplicationStatus.Interview,
                            JobApplicationStatus.Rejected
                        }
                    },
                    {
                        JobApplicationStatus.Interview, new[]
                        {
                            JobApplicationStatus.Accepted,
                            JobApplicationStatus.Rejected
                        }
                    },
                    {
                        JobApplicationStatus.Accepted,
                        Array.Empty<JobApplicationStatus>()
                    },
                    {
                        JobApplicationStatus.Rejected,
                        Array.Empty<JobApplicationStatus>()
                    },
                    {
                        JobApplicationStatus.Cancelled,
                        Array.Empty<JobApplicationStatus>()
                    }
                };

            if (!allowedTransitions[currentStatus].Contains(newStatus))
                throw new BadRequestException($"Cannot change application status from " + $"{currentStatus} to {newStatus}.");
        }
    }
}

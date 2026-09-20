using Application.DTOs.JobApplicationDto;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services
{
    public class JobApplicationService(IUnitOfWork _unitOfWork, IMapper _mapper) : IJobApplicationService
    {
        public async Task<JobApplicationDto> ApplyJobAsync(int jobId, int candidateId)
        {
            var job = await _unitOfWork.GetRepository<Job>().GetByIdAsync(jobId);
            if (job is null)
                throw new NotFoundException("Job not found.");
            if (!job.IsActive)
                throw new BadRequestException("You cannot apply to a closed job.");
            var existingApplication = _unitOfWork.GetRepository<JobApplication>().GetAll().FirstOrDefault(a =>
                    a.JobId == jobId &&
                    a.CandidateId == candidateId &&
                    a.JobApplicationStatus != JobApplicationStatus.Cancelled);
            if (existingApplication is not null)
                throw new BadRequestException("You have already applied to this job.");
            var jobApplication = new JobApplication
            {
                JobId = jobId,
                CandidateId = candidateId,
                JobApplicationStatus = JobApplicationStatus.Applied,
                AppliedAt = DateTime.UtcNow,
                StatusUpdatedAt = DateTime.UtcNow
            };
            await _unitOfWork.GetRepository<JobApplication>().AddAsync(jobApplication);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<JobApplicationDto>(jobApplication);
        }

        public async Task CancelApplication(int candidateId, int applicationId)
        {
            var application = await _unitOfWork.GetRepository<JobApplication>().GetByIdAsync(applicationId);
            if (application is null)
                throw new NotFoundException("Application not found.");
            if (application.CandidateId != candidateId)
                throw new ForbiddenException("You are not the owner of this application.");
            if (application.JobApplicationStatus != JobApplicationStatus.Applied && 
                application.JobApplicationStatus != JobApplicationStatus.UnderReview)
                    throw new BadRequestException("The application is already cancelled.");
            application.JobApplicationStatus = JobApplicationStatus.Cancelled;
            application.CancelledAt = DateTime.UtcNow;
            application.StatusUpdatedAt = DateTime.UtcNow;
            _unitOfWork.GetRepository<JobApplication>().Update(application);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<JobApplicationDto>> GetJobApplicationsAsync(int jobId, int recruiterId)
        {
            var job = await _unitOfWork.GetRepository<Job>().GetByIdAsync(jobId); 
            if (job is null) 
                throw new NotFoundException("Job not found."); 
            if (job.RecruiterId != recruiterId) 
                throw new ForbiddenException("You are not the owner of this job."); 
            var applications = _unitOfWork.GetRepository<JobApplication>().GetAll().Where(a => a.JobId == jobId).ToList(); 
            return _mapper.Map<IEnumerable<JobApplicationDto>>(applications);
        }

        public async Task<JobApplicationDto> GetMyApplicationAsync(int applicationId, int candidateId)
        {
            var application = await _unitOfWork.GetRepository<JobApplication>().GetByIdAsync(applicationId); 
            if (application is null) 
                throw new NotFoundException("Application not found."); 
            if (application.CandidateId != candidateId) 
                throw new ForbiddenException("You are not the owner of this application."); 
            return _mapper.Map<JobApplicationDto>(application);
        }

        public async Task<IEnumerable<JobApplicationDto>> GetMyApplicationsAsync(int candidateId)
        {
            var applications = _unitOfWork.GetRepository<JobApplication>().GetAll().Where(a => a.CandidateId == candidateId).ToList();
            return _mapper.Map<IEnumerable<JobApplicationDto>>(applications);
        }

        public async Task<JobApplicationDto> UpdateApplicationStatusAsync(int applicationId, int recruiterId, UpdateJobApplicationStatusDto jobApplicationStatusDto)
        {
            var application = await _unitOfWork.JobApplicationRepository.GetByIdWithJobAsync(applicationId);
            if (application is null)
                throw new NotFoundException("Application not found.");
            if (application.Job.RecruiterId != recruiterId)
                throw new ForbiddenException("You are not the owner of this job.");
            if (application.JobApplicationStatus == JobApplicationStatus.Cancelled)
                throw new BadRequestException("A cancelled application cannot be updated.");
            ValidateStatusTransition(application.JobApplicationStatus, jobApplicationStatusDto.Status);
            application.JobApplicationStatus = jobApplicationStatusDto.Status;
            application.StatusUpdatedAt = DateTime.UtcNow;
            _unitOfWork.GetRepository<JobApplication>().Update(application);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<JobApplicationDto>(application);
        }

        private void ValidateStatusTransition(JobApplicationStatus currentStatus, JobApplicationStatus newStatus)
        {
            if (currentStatus == newStatus)
                throw new BadRequestException("The application already has this status.");
            if (currentStatus == JobApplicationStatus.Cancelled)
                throw new BadRequestException("A cancelled application cannot be reopened.");
            var allowedTransitions =new Dictionary<JobApplicationStatus, JobApplicationStatus[]>
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

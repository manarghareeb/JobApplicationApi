using Application.DTOs.JobApplicationDto;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Features.JobApplications.Commands.ApplyJobApplicationCommand
{
    internal class ApplyJobApplicationCommandHandler(IUnitOfWork _unitOfWork, IMapper _mapper, ICurrentUserService _currentUserService) 
        : IRequestHandler<ApplyJobApplicationCommand, JobApplicationDto>
    {
        public async Task<JobApplicationDto> Handle(ApplyJobApplicationCommand request, CancellationToken cancellationToken)
        {
            var job = await _unitOfWork.GetRepository<Job>().GetByIdAsync(request.JobId);
            if (job is null)
                throw new NotFoundException("Job not found.");
            if (!job.IsActive)
                throw new BadRequestException("You cannot apply to a closed job.");
            var existingApplication = _unitOfWork.GetRepository<JobApplication>().GetAll().FirstOrDefault(a =>
                    a.JobId == request.JobId &&
                    a.CandidateId == _currentUserService.CandidateId &&
                    a.JobApplicationStatus != JobApplicationStatus.Cancelled);
            if (existingApplication is not null)
                throw new BadRequestException("You have already applied to this job.");
            var jobApplication = new JobApplication
            {
                JobId = request.JobId,
                CandidateId = _currentUserService.CandidateId,
                JobApplicationStatus = JobApplicationStatus.Applied,
                AppliedAt = DateTime.UtcNow,
                StatusUpdatedAt = DateTime.UtcNow
            };
            await _unitOfWork.GetRepository<JobApplication>().AddAsync(jobApplication);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<JobApplicationDto>(jobApplication);
        }
    }
}

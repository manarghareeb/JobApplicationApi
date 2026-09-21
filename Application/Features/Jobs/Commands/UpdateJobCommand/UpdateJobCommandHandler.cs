using Application.DTOs.Jobs;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.Jobs.Commands.UpdateJobCommand
{
    internal class UpdateJobCommandHandler(IUnitOfWork _unitOfWork, IMapper _mapper, 
        ICurrentUserService _currentUserService) : IRequestHandler<UpdateJobCommand, JobDto>
    {
        public async Task<JobDto> Handle(UpdateJobCommand request, CancellationToken cancellationToken)
        {
            var job = await _unitOfWork.GetRepository<Job>().GetByIdAsync(request.JobId);
            if (job is null)
                throw new NotFoundException("Job not found.");
            if (job.RecruiterId != _currentUserService.RecruiterId)
                throw new ForbiddenException(
                    "You are not the owner of this job.");
            if (!job.IsActive)
                throw new BadRequestException(
                    "A closed job cannot be updated.");
            if (request.Title is not null)
                job.Title = request.Title;
            if (request.Description is not null)
                job.Description = request.Description;
            _unitOfWork.GetRepository<Job>().Update(job);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<JobDto>(job);
        }
    }
}

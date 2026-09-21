using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Services;
using Domain.Entities;
using MediatR;

namespace Application.Features.Jobs.Commands.CloseJobCommand
{
    internal class CloseJobCommandHandler(IUnitOfWork _unitOfWork, ICurrentUserService _currentUserService) 
        : IRequestHandler<CloseJobCommand>
    {
        public async Task Handle(CloseJobCommand request, CancellationToken cancellationToken)
        {
            var job = await _unitOfWork.GetRepository<Job>().GetByIdAsync(request.JobId);
            if (job is null)
                throw new NotFoundException("Job not found.");
            if (job.RecruiterId != _currentUserService.RecruiterId)
                throw new ForbiddenException(
                    "You are not the owner of this job.");
            if (!job.IsActive)
                throw new BadRequestException("Job is already closed.");
            job.IsActive = false;
            job.ClosedAt = DateTime.UtcNow;
            _unitOfWork.GetRepository<Job>().Update(job);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}

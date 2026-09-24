using Application.DTOs.Jobs;
using Application.Interfaces;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.Jobs.Commands.CreateJobCommand
{
    internal class CreateJobCommandHandler(IUnitOfWork _unitOfWork, IMapper _mapper, ICurrentUserService _currentUserService,
        IBackgroundJobScheduler _backgroundJobScheduler) : IRequestHandler<CreateJobCommand, JobDto>
    {
        public async Task<JobDto> Handle(CreateJobCommand request, CancellationToken cancellationToken)
        {
            var job = new Job
            {
                Title = request.Title,
                Description = request.Description,
                RecruiterId = _currentUserService.RecruiterId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CloseAt = request.CloseAt
            };
            await _unitOfWork.GetRepository<Job>().AddAsync(job);
            await _unitOfWork.SaveChangesAsync();
            _backgroundJobScheduler.Enqueue<INotificationService>(b => b.NotifyRecruiter(job.Id));
            return _mapper.Map<JobDto>(job);
        }
    }
}

using Application.DTOs.JobApplicationDto;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.JobApplications.Queries.GetJobApplicationsQuery
{
    internal class GetJobApplicationsQueryHandler(IUnitOfWork _unitOfWork, IMapper _mapper, ICurrentUserService _currentUserService) 
        : IRequestHandler<GetJobApplicationsQuery, IEnumerable<JobApplicationDto>>
    {
        public async Task<IEnumerable<JobApplicationDto>> Handle(GetJobApplicationsQuery request, CancellationToken cancellationToken)
        {
            var job = await _unitOfWork.GetRepository<Job>().GetByIdAsync(request.JobId);
            if (job is null)
                throw new NotFoundException("Job not found.");
            if (job.RecruiterId != _currentUserService.RecruiterId)
                throw new ForbiddenException("You are not the owner of this job.");
            var applications = _unitOfWork.GetRepository<JobApplication>().GetAll().Where(a => a.JobId == request.JobId).ToList();
            return _mapper.Map<IEnumerable<JobApplicationDto>>(applications);
        }
    }
}

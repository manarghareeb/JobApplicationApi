using Application.DTOs.JobApplicationDto;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.JobApplications.Queries.GetMyApplicationByIdQuery
{
    internal class GetMyApplicationByIdQueryHandler(IUnitOfWork _unitOfWork, IMapper _mapper, ICurrentUserService _currentUserService) 
        : IRequestHandler<GetMyApplicationByIdQuery, JobApplicationDto>
    {
        public async Task<JobApplicationDto> Handle(GetMyApplicationByIdQuery request, CancellationToken cancellationToken)
        {
            var application = await _unitOfWork.GetRepository<JobApplication>().GetByIdAsync(request.ApplicationId);
            if (application is null)
                throw new NotFoundException("Application not found.");
            if (application.CandidateId != _currentUserService.CandidateId)
                throw new ForbiddenException("You are not the owner of this application.");
            return _mapper.Map<JobApplicationDto>(application);
        }
    }
}

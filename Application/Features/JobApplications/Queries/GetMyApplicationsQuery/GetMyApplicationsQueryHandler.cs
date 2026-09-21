using Application.DTOs.JobApplicationDto;
using Application.Interfaces;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.JobApplications.Queries.GetMyApplicationsQuery
{
    internal class GetMyApplicationsQueryHandler(IUnitOfWork _unitOfWork, IMapper _mapper, ICurrentUserService _currentUserService) 
        : IRequestHandler<GetMyApplicationsQuery, IEnumerable<JobApplicationDto>>
    {
        public async Task<IEnumerable<JobApplicationDto>> Handle(GetMyApplicationsQuery request, CancellationToken cancellationToken)
        {
            var applications = _unitOfWork.GetRepository<JobApplication>().GetAll()
                .Where(a => a.CandidateId == _currentUserService.CandidateId).ToList();
            return _mapper.Map<IEnumerable<JobApplicationDto>>(applications);
        }
    }
}

using Application.DTOs.Jobs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.Jobs.Queries.GetAllJobsQuery
{
    internal class GetAllJobsQueryHandler(IUnitOfWork _unitOfWork, IMapper _mapper) : IRequestHandler<GetAllJobsQuery, IEnumerable<JobDto>>
    {
        public async Task<IEnumerable<JobDto>> Handle(GetAllJobsQuery request, CancellationToken cancellationToken)
        {
            var jobs = _unitOfWork.GetRepository<Job>().GetAll().Where(j => j.IsActive).ToList();
            return _mapper.Map<IEnumerable<JobDto>>(jobs);
        }
    }
}

using Application.DTOs.Jobs;
using MediatR;

namespace Application.Features.Jobs.Queries.GetAllJobsQuery
{
    public record GetAllJobsQuery : IRequest<IEnumerable<JobDto>>;
}

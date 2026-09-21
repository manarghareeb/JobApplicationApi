using Application.DTOs.JobApplicationDto;
using MediatR;

namespace Application.Features.JobApplications.Queries.GetMyApplicationsQuery
{
    public record GetMyApplicationsQuery : IRequest<IEnumerable<JobApplicationDto>>;
}

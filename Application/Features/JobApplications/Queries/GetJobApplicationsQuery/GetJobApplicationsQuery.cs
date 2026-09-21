using Application.DTOs.JobApplicationDto;
using MediatR;

namespace Application.Features.JobApplications.Queries.GetJobApplicationsQuery
{
    public record GetJobApplicationsQuery(int JobId) : IRequest<IEnumerable<JobApplicationDto>>;
}

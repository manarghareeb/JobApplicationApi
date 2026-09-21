using Application.DTOs.JobApplicationDto;
using MediatR;

namespace Application.Features.JobApplications.Queries.GetMyApplicationByIdQuery
{
    public record GetMyApplicationByIdQuery(int ApplicationId) : IRequest<JobApplicationDto>;
}

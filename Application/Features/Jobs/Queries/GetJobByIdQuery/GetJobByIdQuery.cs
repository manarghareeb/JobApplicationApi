using Application.DTOs.Jobs;
using MediatR;

namespace Application.Features.Jobs.Queries.GetJobByIdQuery
{
    public record GetJobByIdQuery(int JobId) : IRequest<JobDto>;
}

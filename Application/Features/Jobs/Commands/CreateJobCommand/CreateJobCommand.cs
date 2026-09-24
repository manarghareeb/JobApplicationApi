using Application.DTOs.Jobs;
using MediatR;

namespace Application.Features.Jobs.Commands.CreateJobCommand
{
    public record CreateJobCommand(string Title, string Description, DateTime? CloseAt) : IRequest<JobDto>;
}

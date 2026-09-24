using Application.DTOs.Jobs;
using MediatR;

namespace Application.Features.Jobs.Commands.UpdateJobCommand
{
    public record UpdateJobCommand(int JobId, string? Title, string? Description, DateTime? CloseAt) : IRequest<JobDto>;
}

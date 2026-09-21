using MediatR;

namespace Application.Features.Jobs.Commands.CloseJobCommand
{
    public record CloseJobCommand(int JobId) : IRequest;
}

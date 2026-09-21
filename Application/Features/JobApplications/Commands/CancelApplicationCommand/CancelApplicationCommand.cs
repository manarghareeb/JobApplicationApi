using MediatR;

namespace Application.Features.JobApplications.Commands.CancelApplicationCommand
{
    public record CancelApplicationCommand(int Id) : IRequest;
}

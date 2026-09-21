using Application.DTOs.JobApplicationDto;
using MediatR;

namespace Application.Features.JobApplications.Commands.UpdateApplicationStatusCommand
{
    public record UpdateApplicationStatusCommand(int ApplicationId, UpdateJobApplicationStatusDto UpdateJobApplicationStatusDto) 
        : IRequest<JobApplicationDto>;
}

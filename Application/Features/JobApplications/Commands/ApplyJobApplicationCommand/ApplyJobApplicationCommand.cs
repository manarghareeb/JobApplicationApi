using Application.DTOs.JobApplicationDto;
using MediatR;

namespace Application.Features.JobApplications.Commands.ApplyJobApplicationCommand
{
    public record ApplyJobApplicationCommand(int JobId) : IRequest<JobApplicationDto>;
}

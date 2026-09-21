using Application.DTOs.JobApplicationDto;
using Application.Features.JobApplications.Commands.ApplyJobApplicationCommand;
using Application.Features.JobApplications.Commands.CancelApplicationCommand;
using Application.Features.JobApplications.Commands.UpdateApplicationStatusCommand;
using Application.Features.JobApplications.Queries.GetJobApplicationsQuery;
using Application.Features.JobApplications.Queries.GetMyApplicationByIdQuery;
using Application.Features.JobApplications.Queries.GetMyApplicationsQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobApplication.API.Controllers
{
    [ApiController]
    [Route("api")]
    public class ApplicationsController(IMediator mediator) : ControllerBase
    {
        [HttpPost("jobs/{jobId:int}/applications")]
        [Authorize(Roles = "Candidate")]
        public async Task<ActionResult<JobApplicationDto>> ApplyJobApplication(int jobId)
            => Ok(await mediator.Send(new ApplyJobApplicationCommand(jobId)));

        [HttpGet("jobs/{jobId:int}/applications")]
        [Authorize(Roles = "Recruiter")]
        public async Task<ActionResult<IEnumerable<JobApplicationDto>>> GetJobApplications(int jobId)
            => Ok(await mediator.Send(new GetJobApplicationsQuery(jobId)));
        
        [HttpPatch("jobs/{jobId:int}/applications/{id:int}")]
        [Authorize(Roles = "Recruiter")]
        public async Task<ActionResult<JobApplicationDto>>UpdateApplicationStatus(int id, UpdateJobApplicationStatusDto dto)
            => Ok(await mediator.Send(new UpdateApplicationStatusCommand(id, dto)));
        
        [HttpGet("applications/me/{id:int}")]
        [Authorize(Roles = "Candidate")]
        public async Task<ActionResult<JobApplicationDto>>GetMyApplicationById(int id)
            => Ok(await mediator.Send(new GetMyApplicationByIdQuery(id)));
        
        [HttpGet("applications/me")]
        [Authorize(Roles = "Candidate")]
        public async Task<ActionResult<IEnumerable<JobApplicationDto>>>GetMyApplications()
            => Ok(await mediator.Send(new GetMyApplicationsQuery()));
        
        [HttpDelete("applications/{id:int}")]
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> CancelApplication(int id, CancellationToken token)
        {
            await mediator.Send(new CancelApplicationCommand(id), token);
            return NoContent();
        }
    }
}
using Application.DTOs.Jobs;
using Application.Features.Jobs.Commands.CloseJobCommand;
using Application.Features.Jobs.Commands.CreateJobCommand;
using Application.Features.Jobs.Commands.UpdateJobCommand;
using Application.Features.Jobs.Queries.GetAllJobsQuery;
using Application.Features.Jobs.Queries.GetJobByIdQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobApplication.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobsController(IMediator mediator) : ControllerBase
    {
        [HttpPost]
        [Authorize(Roles = "Recruiter")]
        public async Task<ActionResult<JobDto>> CreateJob(CreateJobDto createJobDto)
            => Ok(await mediator.Send(new CreateJobCommand(createJobDto.Title, createJobDto.Description, createJobDto.CloseAt)));

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<JobDto>>> GetAllJobs()
            => Ok(await mediator.Send(new GetAllJobsQuery()));

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<ActionResult<JobDto>> GetJobById(int id)
            => Ok(await mediator.Send(new GetJobByIdQuery(id)));
        
        [HttpPatch("{id:int}")]
        [Authorize(Roles = "Recruiter")]
        public async Task<ActionResult<JobDto>> UpdateJob(int id, UpdateJobDto updateJobDto)
            => Ok(await mediator.Send(new UpdateJobCommand(id, updateJobDto.Title, updateJobDto.Description, updateJobDto.CloseAt)));

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> CloseJob(int id)
        {
            await mediator.Send(new CloseJobCommand(id));
            return NoContent();
        }
    }
}

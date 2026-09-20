using Application.DTOs.Jobs;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobApplication.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobsController(IServiceManager serviceManager) : ControllerBase
    {
        [HttpPost]
        [Authorize(Roles = "Recruiter")]
        public async Task<ActionResult<JobDto>> CreateJob(CreateJobDto createJobDto)
        {
            var recruiterId = serviceManager.CurrentUserService.RecruiterId;
            return Ok(await serviceManager.JobService.CreateJobAsync(createJobDto, recruiterId));
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<JobDto>>> GetAllJobs()
            => Ok(await serviceManager.JobService.GetAllJobsAsync());

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<ActionResult<JobDto>> GetJobById(int id)
            => Ok(await serviceManager.JobService.GetJobByIdAsync(id));
        
        [HttpPatch("{id:int}")]
        [Authorize(Roles = "Recruiter")]
        public async Task<ActionResult<JobDto>> UpdateJob(int id, UpdateJobDto updateJobDto)
        {
            var recruiterId = serviceManager.CurrentUserService.RecruiterId;
            return Ok(await serviceManager.JobService.UpdateJobAsync(id, updateJobDto, recruiterId));
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> CloseJob(int id)
        {
            var recruiterId = serviceManager.CurrentUserService.RecruiterId;
            await serviceManager.JobService.CloseJobAsync(id, recruiterId);
            return NoContent();
        }
    }
}

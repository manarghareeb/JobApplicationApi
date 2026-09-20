using Application.DTOs.JobApplicationDto;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobApplication.API.Controllers
{
    [ApiController]
    [Route("api")]
    public class ApplicationsController(IServiceManager serviceManager) : ControllerBase
    {
        [HttpPost("jobs/{jobId:int}/applications")]
        [Authorize(Roles = "Candidate")]
        public async Task<ActionResult<JobApplicationDto>> ApplyJob(int jobId)
        {
            var candidateId = serviceManager.CurrentUserService.CandidateId;
            return Ok(await serviceManager.JobApplicationService.ApplyJobAsync(jobId, candidateId));
        }

        [HttpGet("jobs/{jobId:int}/applications")]
        [Authorize(Roles = "Recruiter")]
        public async Task<ActionResult<IEnumerable<JobApplicationDto>>> GetJobApplications(int jobId)
        {
            var recruiterId = serviceManager.CurrentUserService.RecruiterId;
            var applications = await serviceManager.JobApplicationService.GetJobApplicationsAsync(jobId, recruiterId);
            return Ok(applications);
        }

        [HttpPatch("jobs/{jobId:int}/applications/{id:int}")]
        [Authorize(Roles = "Recruiter")]
        public async Task<ActionResult<JobApplicationDto>>UpdateApplicationStatus(int jobId, int id, UpdateJobApplicationStatusDto dto)
        {
            var recruiterId = serviceManager.CurrentUserService.RecruiterId;
            return Ok(await serviceManager.JobApplicationService.UpdateApplicationStatusAsync(id, recruiterId, dto));
        }

        [HttpGet("applications/{id:int}")]
        [Authorize(Roles = "Candidate")]
        public async Task<ActionResult<JobApplicationDto>>GetMyApplication(int id)
        {
            var candidateId = serviceManager.CurrentUserService.CandidateId;
            return Ok(await serviceManager.JobApplicationService.GetMyApplicationAsync(id, candidateId));
        }

        [HttpGet("applications/me")]
        [Authorize(Roles = "Candidate")]
        public async Task<ActionResult<IEnumerable<JobApplicationDto>>>GetMyApplications()
        {
            var candidateId = serviceManager.CurrentUserService.CandidateId;
            return Ok(await serviceManager.JobApplicationService.GetMyApplicationsAsync(candidateId));
        }

        [HttpDelete("applications/{id:int}")]
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> CancelApplication(int id)
        {
            var candidateId = serviceManager.CurrentUserService.CandidateId;
            await serviceManager.JobApplicationService.CancelApplication(candidateId, id);
            return NoContent();
        }
    }
}
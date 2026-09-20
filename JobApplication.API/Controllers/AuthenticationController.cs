using Application.DTOs.IdentityDto;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace JobApplication.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController(IServiceManager _serviceManager) : ControllerBase
    {
        [HttpPost("Login")]
        public async Task<ActionResult<UserResultDto>> LoginAsync(LoginDto loginDto)
            => Ok(await _serviceManager.AuthenticationService.LoginAsync(loginDto));
        [HttpPost("Register")]
        public async Task<ActionResult<UserResultDto>> RegisterAsync(RegisterDto registerDto)
            => Ok(await _serviceManager.AuthenticationService.RegisterAsync(registerDto));
    }
}
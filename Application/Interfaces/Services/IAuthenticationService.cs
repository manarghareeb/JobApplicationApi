using Application.DTOs.IdentityDto;

namespace Application.Interfaces.Services
{
    public interface IAuthenticationService
    {
        Task<UserResultDto> LoginAsync(LoginDto loginDto);
        Task<UserResultDto> RegisterAsync(RegisterDto registerDto);
    }
}

using Application.DTOs.IdentityDto;
using Application.Exceptions;
using Application.Interfaces.Services;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Identity
{
    public class AuthenticationService(UserManager<ApplicationUser> userManager, IOptions<JwtSettings> jwtSettings
        , RoleManager<IdentityRole<Guid>> roleManager, ApplicationDbContext dbContext) : IAuthenticationService
    {
        public async Task<UserResultDto> LoginAsync(LoginDto loginDto)
        {
            var user = await userManager.FindByEmailAsync(loginDto.Email);
            if (user is null) throw new UnauthorizedException("Invalid email or password");
            var result = await userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!result) throw new UnauthorizedException("Invalid email or password.");
            return new UserResultDto() {
                Email = user.Email,
                Name = user.UserName,
                Token = await CreateTokenAsync(user),
            };
        }

        public async Task<UserResultDto> RegisterAsync(RegisterDto registerDto)
        {
            var role = registerDto.Role.Trim();
            if (role != ApplicationRoles.Candidate && role != ApplicationRoles.Recruiter)
                throw new BadRequestException("Role must be Candidate or Recruiter.");
            var existingUser = await userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser is not null)
                throw new BadRequestException("A user with this email already exists.");
            if (!await roleManager.RoleExistsAsync(role))
                throw new BadRequestException($"Role '{role}' does not exist.");
            await using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                var user = new ApplicationUser
                {
                    Email = registerDto.Email,
                    UserName = registerDto.Name
                };
                var result = await userManager.CreateAsync(user, registerDto.Password);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    throw new ValidationException(errors);
                }
                var roleResult = await userManager.AddToRoleAsync(user, role);
                if (!roleResult.Succeeded)
                {
                    var errors = roleResult.Errors.Select(e => e.Description).ToList();
                    throw new ValidationException(errors);
                }
                if (role == ApplicationRoles.Candidate)
                {
                    if (string.IsNullOrWhiteSpace(registerDto.CvUrl))
                        throw new BadRequestException("CvUrl is required for Candidate.");
                    var candidate = new Candidate
                    {
                        UserId = user.Id,
                        Name = registerDto.Name,
                        Email = registerDto.Email,
                        CvUrl = registerDto.CvUrl,
                        CreatedAt = DateTime.UtcNow
                    };
                    await dbContext.Candidates.AddAsync(candidate);
                }
                else
                {
                    var recruiter = new Recruiter
                    {
                        UserId = user.Id,
                        Name = registerDto.Name,
                        Email = registerDto.Email,
                        CreatedAt = DateTime.UtcNow
                    };
                    await dbContext.Recruiters.AddAsync(recruiter);
                }
                await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return new UserResultDto
                {
                    Email = user.Email,
                    Name = registerDto.Name,
                    Token = await CreateTokenAsync(user)
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private async Task<string> CreateTokenAsync(ApplicationUser user)
        {
            var jwtOptions = jwtSettings.Value;
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            };
            var roles = await userManager.GetRolesAsync(user);
            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key));
            var signInCreds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(issuer: jwtOptions.Issuer, audience: jwtOptions.Audience, claims: claims,
                expires: DateTime.UtcNow.AddMinutes(jwtOptions.ExpirationInMinutes), signingCredentials: signInCreds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

namespace Application.DTOs.IdentityDto
{
    public class UserResultDto
    {
        public string Token { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}

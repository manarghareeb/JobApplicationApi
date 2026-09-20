using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.IdentityDto
{
    public class RegisterDto
    {
        public string Name { get; set; } = null!;
        [EmailAddress]
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string? CvUrl { get; set; }
    }
}


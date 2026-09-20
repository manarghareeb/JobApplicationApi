using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity
{
    public static class IdentitySeeder
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole<Guid>> roleManager)
        {
            var roles = new[]
            {
                ApplicationRoles.Candidate,
                ApplicationRoles.Recruiter
            };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(role));
                }
            }
        }
    }
}

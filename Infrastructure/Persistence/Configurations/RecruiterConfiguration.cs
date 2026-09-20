using Domain.Entities;
using Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class RecruiterConfiguration : IEntityTypeConfiguration<Recruiter>
    {
        public void Configure(EntityTypeBuilder<Recruiter> builder)
        {
            builder.HasKey(r => r.Id);
            builder.HasOne<ApplicationUser>().WithOne().HasForeignKey<Recruiter>(r => r.UserId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}

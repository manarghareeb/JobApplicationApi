using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    internal class JobConfiguration : IEntityTypeConfiguration<Job>
    {
        public void Configure(EntityTypeBuilder<Job> builder)
        {
            builder.HasKey(j => j.Id);
            builder.HasOne(j => j.Recruiter)
                   .WithMany(r => r.Jobs)
                   .HasForeignKey(j => j.RecruiterId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

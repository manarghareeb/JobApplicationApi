using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    internal class JobApplicationConfiguration : IEntityTypeConfiguration<JobApplication>
    {
        public void Configure(EntityTypeBuilder<JobApplication> builder)
        {
            builder.HasKey(a => a.Id);
            builder.HasOne(a => a.Job)
                   .WithMany(j => j.Applications)
                   .HasForeignKey(a => a.JobId)
                   .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(a => a.Candidate)
                   .WithMany(c => c.Applications)
                   .HasForeignKey(a => a.CandidateId)
                   .OnDelete(DeleteBehavior.Restrict);
            builder.HasIndex(a => new { a.CandidateId, a.JobId }).IsUnique().HasFilter("[JobApplicationStatus] <> 5");
        }
    }
}

using Domain.Entities;
using Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class CandidateConfiguration : IEntityTypeConfiguration<Candidate>
    {
        public void Configure(EntityTypeBuilder<Candidate> builder)
        {
            builder.HasKey(c => c.Id);
            builder.HasOne<ApplicationUser>().WithOne().HasForeignKey<Candidate>(c => c.UserId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}

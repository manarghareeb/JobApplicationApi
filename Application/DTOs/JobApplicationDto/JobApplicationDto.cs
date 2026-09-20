using Domain.Enums;

namespace Application.DTOs.JobApplicationDto
{
    public class JobApplicationDto
    {
        public int Id { get; set; }
        public JobApplicationStatus JobApplicationStatus { get; set; }
        public DateTime AppliedAt { get; set; }
        public DateTime StatusUpdatedAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public int CandidateId { get; set; }
        public int JobId { get; set; }
    }
}

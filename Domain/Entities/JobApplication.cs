using Domain.Enums;

namespace Domain.Entities
{
    public class JobApplication
    {
        public int Id { get; set; }
        public JobApplicationStatus JobApplicationStatus { get; set; }
        public DateTime AppliedAt { get; set; }
        public DateTime StatusUpdatedAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public int CandidateId { get; set; }
        public Candidate Candidate { get; set; }
        public int JobId { get; set; }
        public Job Job { get; set; }
    }
}

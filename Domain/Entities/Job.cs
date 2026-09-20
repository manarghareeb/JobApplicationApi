namespace Domain.Entities
{
    public class Job
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public Recruiter Recruiter { get; set; }
        public int RecruiterId { get; set; }
        public ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
    }
}

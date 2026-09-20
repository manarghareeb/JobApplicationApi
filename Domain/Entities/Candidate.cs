namespace Domain.Entities
{
    public class Candidate
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string CvUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
    }
}

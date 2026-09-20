namespace Domain.Entities
{
    public class Recruiter
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<Job> Jobs { get; set; } = new List<Job>();
    }
}

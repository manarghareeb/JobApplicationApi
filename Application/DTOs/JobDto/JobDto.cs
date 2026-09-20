using Domain.Entities;

namespace Application.DTOs.Jobs
{
    public class JobDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public int RecruiterId { get; set; }
    }
}

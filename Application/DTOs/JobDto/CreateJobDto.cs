namespace Application.DTOs.Jobs
{
    public class CreateJobDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? CloseAt { get; set; }
    }
}

namespace TODOAPI.DTOs
{
    public class TodoUpdateDto
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime? CompletedAt { get; set; }
    }
}

namespace TODOAPI.DTOs
{
    public class TodoUpdateDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; } 
        public DateTime? CompletedAt { get; set; }
    }
}

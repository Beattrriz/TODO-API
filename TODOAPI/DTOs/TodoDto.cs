using System.ComponentModel.DataAnnotations;

namespace TODOAPI.DTOs
{
    public class TodoDto
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }
    }
}

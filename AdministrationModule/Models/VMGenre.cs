using System.ComponentModel.DataAnnotations;

namespace AdminModule.Models
{
    public class VMGenre
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Description { get; set; }
    }
}

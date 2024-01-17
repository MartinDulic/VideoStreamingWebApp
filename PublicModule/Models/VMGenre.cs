using System.ComponentModel.DataAnnotations;

namespace PublicModule.Models
{
    public class VMGenre
    {
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Description { get; set; }

    }
}

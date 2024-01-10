using System.ComponentModel.DataAnnotations;

namespace AdminModule.Models
{
    public class VMVideo
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        [Required]
        public string? Name { get; set; } = null!;
        [Required]
        public string? Description { get; set; }
        [Display(Name = "Genre")]
        [Required]
        public int GenreId { get; set; }
        [Display(Name = "Time(in seconds)")]
        [Required]
        public int TotalSeconds { get; set; }
        [Display(Name = "Streaming URL")]
        [Url(ErrorMessage = "Invalid URL format")]
        [Required]
        public string? StreamingUrl { get; set; }
        [Display(Name = "Image")]
        [Required]
        public int ImageId { get; set; }
        public virtual VMGenre Genre { get; set; } = null!;
        public virtual VMImage? Image { get; set; }

        public virtual ICollection<VMVideoTag>? VideoTags { get; set; } = new List<VMVideoTag>();
        [Display(Name = "Tags")]
        public string NewTags { get; set; }

    }
}

using DAL.Model;
using System.ComponentModel.DataAnnotations;

namespace PublicModule.Models
{
    public class VMVideo
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Name { get; set; } = null!;
        public string? Description { get; set; }
        public int TotalSeconds { get; set; }
        public string? StreamingUrl { get; set; }
        public virtual Genre Genre { get; set; } = null!;
        public virtual Image? Image { get; set; }

        public virtual ICollection<VMPublicVideoTag> VideoTags { get; set; } = new List<VMPublicVideoTag>();
        [Display(Name = "Tags")]
        public string NewTags { get; set; }
    }
}

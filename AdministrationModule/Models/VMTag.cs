using System.ComponentModel.DataAnnotations;

namespace AdministrationModule.Models
{
    public class VMTag
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;
    }
}

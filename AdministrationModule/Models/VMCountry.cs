using System.ComponentModel.DataAnnotations;

namespace AdministrationModule.Models
{
    public class VMCountry
    {
        public int Id { get; set; }
        [Required]
        public string Code { get; set; } = null!;
        [Required]
        public string Name { get; set; } = null!;
    }
}

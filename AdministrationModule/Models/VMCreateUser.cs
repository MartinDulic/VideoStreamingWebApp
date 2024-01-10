using System.ComponentModel.DataAnnotations;

namespace AdminModule.Models
{
    public class VMCreateUser
    {
        [Required]
        [Display(Name = "First name")]
        public string? FirstName { get; set; }
        [Required]
        [Display(Name = "Last name")]
        public string? LastName { get; set; }
        [Required]
        public string? Username { get; set; }
        [Required]
        public string? Password { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        public string? Phone { get; set; }
        [Required]
        public int Country { get; set; }
    }
}

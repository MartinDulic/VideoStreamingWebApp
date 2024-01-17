using System.ComponentModel.DataAnnotations;

namespace PublicModule.Models
{
    public class VMUserRegister
    {
        [Required]
        [Display(Name = "First Name:")]
        public string? FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name:")]
        public string? LastName { get; set; }
        [Required]
        [Display(Name = "Username:")]
        public string? Username { get; set; }
        [Required]
        [Display(Name = "Password:")]
        public string? Password { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "Email:")]
        public string? Email { get; set; }
        [Display(Name = "Phone number:")]
        public string? Phone { get; set; }
        [Required]
        [Display(Name = "Country of Residence:")]
        public int Country { get; set; }
    }
}

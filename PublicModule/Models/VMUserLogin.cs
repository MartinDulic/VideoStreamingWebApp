using System.ComponentModel.DataAnnotations;

namespace PublicModule.Models
{
    public class VMUserLogin
    {
        [Required]
        [Display(Name = "Username:")]
        public string? Username { get; set; }
        [Required]
        [Display(Name = "Password:")]
        public string? Password { get; set; }
        public bool StaySignedIn { get; set; }
        public string? RedirectUrl { get; set; }
    }
}

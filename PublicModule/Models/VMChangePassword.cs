using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PublicModule.Models
{
    public class VMChangePassword
    {
        [Required]
        [DisplayName("User name")]
        public string Username { get; set; }
        [Required]
        [DisplayName("Current Password")]
        public string Password { get; set; }
        [Required]
        [DisplayName("New Password")]
        public string NewPassword { get; set; }

    }
}

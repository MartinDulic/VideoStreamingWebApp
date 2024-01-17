using Microsoft.Build.Framework;

namespace PublicModule.Models
{
    public class VMValidateEmail
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string SecurityToken { get; set; }
    }
}

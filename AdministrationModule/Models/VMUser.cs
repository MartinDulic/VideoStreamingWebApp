using System.ComponentModel.DataAnnotations;

namespace AdminModule.Models
{
    public class VMUser
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string Username { get; set; } = null!;
        [Display(Name = "First Name:")]
        public string FirstName { get; set; } = null!;
        [Display(Name = "Last Name:")]
        public string LastName { get; set; } = null!;
        [Display(Name = "Email:")]
        public string Email { get; set; } = null!;
        public string PwdHash { get; set; } = null!;
        public string PwdSalt { get; set; } = null!;
        [Display(Name = "Phone number:")]
        public string? Phone { get; set; }
        [Display(Name = "Activate/Disable account")]
        public bool IsConfirmed { get; set; }
        public string? SecurityToken { get; set; }
        [Display(Name = "Country of residence:")]
        public int CountryOfResidenceId { get; set; }
        public virtual VMCountry CountryOfResidence { get; set; } = null!;
    }
}

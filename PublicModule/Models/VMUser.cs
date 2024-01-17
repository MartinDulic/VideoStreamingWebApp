using System.ComponentModel.DataAnnotations;

namespace PublicModule.Models
{
    public class VMUser
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        [Display(Name = "First Name:")]
        public string FirstName { get; set; } = null!;
        [Display(Name = "Last Name:")]
        public string LastName { get; set; } = null!;
        [Display(Name = "Email:")]
        public string Email { get; set; } = null!;
        [Display(Name = "Phone number:")]
        public string? Phone { get; set; }
        [Display(Name = "Country of residence:")]
        public int CountryOfResidenceId { get; set; }
        [Display(Name = "Country of residence:")]
        public virtual VMCountry CountryOfResidence { get; set; } = null!;
    }
}

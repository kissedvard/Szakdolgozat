using Microsoft.AspNetCore.Identity;

namespace OkosBufeWeb.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName {get; set;}
        public string? Description {get; set;}
        public DateTime? BirthDate {get; set;}

    }
}
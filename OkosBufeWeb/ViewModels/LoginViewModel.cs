using System.ComponentModel.DataAnnotations;

namespace OkosBufeWeb.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Az e-mail cím megadása kötelező")]
        [EmailAddress(ErrorMessage = "Érvénytelen e-mail formátum!")]
        [Display(Name = "E-mail cím")]
        public string Email {get; set;} = string.Empty;

        [Required(ErrorMessage ="A jelszó megadása kötelező")]
        [DataType(DataType.Password)]
        [Display(Name = "Jelszó")]
        public string Password {get; set;} = string.Empty;

        [Display(Name = "Emlékezz rám")]
        public bool RememberMe {get; set;}
    }
}
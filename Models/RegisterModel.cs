using System.ComponentModel.DataAnnotations;

namespace BibliotekaOnline.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Email jest wymagany")]
        [EmailAddress(ErrorMessage = "Nieprawidłowy format adresu email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hasło jest wymagane")]
        [StringLength(100, ErrorMessage = "Hasło musi mieć co najmniej {2} znaków długości.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Potwierdź hasło")]
        [Compare("Password", ErrorMessage = "Hasła nie są identyczne.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
} 
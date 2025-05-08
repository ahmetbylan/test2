using System.ComponentModel.DataAnnotations;

namespace B2BUygulamasi.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "E-posta zorunludur")]
        [EmailAddress(ErrorMessage = "Geçersiz e-posta formatı")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Şifre zorunludur")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Display(Name = "Beni Hatırla")]
        public bool RememberMe { get; set; }
    }
}
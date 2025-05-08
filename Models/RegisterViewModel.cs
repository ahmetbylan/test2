using System.ComponentModel.DataAnnotations;

namespace B2BUygulamasi.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Ad zorunludur")]
        public string Ad { get; set; } = null!;

        [Required(ErrorMessage = "Soyad zorunludur")]
        public string Soyad { get; set; } = null!;

        [Required(ErrorMessage = "E-posta zorunludur")]
        [EmailAddress(ErrorMessage = "Geçersiz e-posta formatı")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Şifre zorunludur")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6)]
        public string Sifre { get; set; } = null!;

        [DataType(DataType.Password)]
        [Compare("Sifre", ErrorMessage = "Şifreler uyuşmuyor")]
        public string SifreTekrar { get; set; } = null!;

        [Required(ErrorMessage = "Firma adı zorunludur")]
        public string FirmaAdi { get; set; } = null!;
    }
}
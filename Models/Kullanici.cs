using System.ComponentModel.DataAnnotations;

namespace B2BUygulamasi.Models
{
    public class Kullanici
    {
        [Key]
        public int KullaniciID { get; set; }

        [Required(ErrorMessage = "Ad zorunludur")]
        public string Ad { get; set; } = null!;

        [Required(ErrorMessage = "Soyad zorunludur")]
        public string Soyad { get; set; } = null!;

        [Required(ErrorMessage = "E-posta zorunludur")]
        [EmailAddress(ErrorMessage = "Geçersiz e-posta formatı")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Şifre zorunludur")]
        public string Sifre { get; set; } = null!;

        public string? FirmaAdi { get; set; }
        public DateTime KayitTarihi { get; set; }
        public DateTime? SonGirisTarihi { get; set; }
        public bool Aktif { get; set; } = true;
        public virtual ICollection<KullaniciRol> KullaniciRolleri { get; set; } = new List<KullaniciRol>();
    }
}
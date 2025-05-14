using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [DataType(DataType.Password)]
        public string Sifre { get; set; } = null!;

        // Firma ilişkisi için
        [ForeignKey("Firma")]
        public int? FirmaID { get; set; }
        public virtual Firma? Firma { get; set; }

        // Eski FirmaAdi alanını kaldırabilirsiniz (Firma nesnesi üzerinden erişilebilir)
        public DateTime KayitTarihi { get; set; }
        public DateTime? SonGirisTarihi { get; set; }
        public bool Aktif { get; set; } = true;

        public virtual ICollection<KullaniciRol> KullaniciRolleri { get; set; } = new List<KullaniciRol>();
    }

    public class Firma
    {
        [Key]
        public int FirmaID { get; set; }

        [Required]
        public string FirmaAdi { get; set; } = null!;

        [Display(Name = "Birinci İskonto (%)")]
        [Range(0, 100, ErrorMessage = "İskonto 0-100 arasında olmalıdır")]
        public double BirinciIskontoOrani { get; set; } = 15; // Varsayılan değer

        [Display(Name = "İkinci İskonto (%)")]
        [Range(0, 100, ErrorMessage = "İskonto 0-100 arasında olmalıdır")]
        public double IkinciIskontoOrani { get; set; } = 5; // Varsayılan değer

        public virtual ICollection<Kullanici> Kullanicilar { get; set; } = new List<Kullanici>();
    }
}
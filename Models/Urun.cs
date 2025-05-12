using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2BUygulamasi.Models
{
    public class Urun
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UrunID { get; set; }

        [Required(ErrorMessage = "Ürün adı zorunludur")]
        [StringLength(100, ErrorMessage = "Ürün adı en fazla 100 karakter olabilir")]
        [Display(Name = "Ürün Adı")]
        public string UrunAdi { get; set; }

        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir")]
        [DataType(DataType.MultilineText)]
        public string Aciklama { get; set; }

        [Required(ErrorMessage = "Fiyat zorunludur")]
        [Column(TypeName = "decimal(18,6)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Fiyat 0'dan büyük olmalıdır")]
        [Display(Name = "Fiyat")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Fiyat { get; set; }

        [Required(ErrorMessage = "Stok miktarı zorunludur")]
        [Range(0, int.MaxValue, ErrorMessage = "Stok miktarı negatif olamaz")]
        [Display(Name = "Stok Miktarı")]
        public int StokMiktari { get; set; }

        [Required(ErrorMessage = "Kategori zorunludur")]
        [StringLength(50, ErrorMessage = "Kategori en fazla 50 karakter olabilir")]
        [Display(Name = "Kategori")]
        public string Kategori { get; set; }

        [StringLength(255, ErrorMessage = "Resim yolu çok uzun")]
        [Display(Name = "Resim Yolu")]
        public string ResimYolu { get; set; }

        // Ek özellikler (opsiyonel)
        //[Display(Name = "Oluşturulma Tarihi")]
        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        //public DateTime OlusturmaTarihi { get; set; } = DateTime.UtcNow;

        [Display(Name = "Aktif Mi?")]
        public bool AktifMi { get; set; } = true;
    }
}
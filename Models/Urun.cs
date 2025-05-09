using Microsoft.AspNetCore.Mvc;

namespace B2BUygulamasi.Models
{
    public class Urun
    {
        public int UrunID { get; set; }
        public string UrunAdi { get; set; }
        public string Aciklama { get; set; }
        public decimal Fiyat { get; set; }
        public int StokMiktari { get; set; }
        public string Kategori { get; set; }
        public string ResimYolu { get; set; }
    }
}
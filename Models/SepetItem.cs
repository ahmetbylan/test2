namespace B2BUygulamasi.Models
{
    public class SepetItem
    {
        public int UrunId { get; set; }
        public string UrunAdi { get; set; }
        public decimal BirimFiyat { get; set; }
        public int Adet { get; set; }
        public string ResimUrl { get; set; }
        public decimal ToplamTutar => BirimFiyat * Adet;
    }
}
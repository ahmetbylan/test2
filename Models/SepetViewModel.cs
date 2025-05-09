public class SepetViewModel
{
    public int UrunId { get; set; }
    public string UrunAdi { get; set; }
    public decimal BirimFiyat { get; set; }
    public int Adet { get; set; }
    public decimal ToplamTutar => BirimFiyat * Adet;
    public string ResimUrl { get; set; }
}
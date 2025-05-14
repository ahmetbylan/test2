using B2BUygulamasi.Models;

namespace B2BUygulamasi.Services
{
    public class FiyatHesaplamaService
    {
        public decimal IskontoluFiyatHesapla(decimal orjinalFiyat, Firma firma)
        {
            decimal birinciIskonto = orjinalFiyat * (decimal)(firma.BirinciIskontoOrani / 100);
            decimal araFiyat = orjinalFiyat - birinciIskonto;

            decimal ikinciIskonto = araFiyat * (decimal)(firma.IkinciIskontoOrani / 100);
            return araFiyat - ikinciIskonto;
        }

        // Ekstra fiyat formatlama metodu
        public string FormatliFiyat(double fiyat)
        {
            return fiyat.ToString("C2", new System.Globalization.CultureInfo("tr-TR"));
        }
    }
}
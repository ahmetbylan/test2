using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace B2BUygulamasi.Models
{
    public class Siparis
    {
        public int SiparisId { get; set; }
        public string KullaniciId { get; set; }
        public DateTime SiparisTarihi { get; set; }
        public decimal ToplamTutar { get; set; }
        public string Durum { get; set; } // "Hazırlanıyor", "Kargoda", "Tamamlandı"

        // Navigation property
        public ICollection<SiparisDetay> SiparisDetaylar { get; set; }
    }

    public class SiparisDetay
    {
        public int SiparisDetayId { get; set; }
        public int SiparisId { get; set; }
        public int UrunId { get; set; }
        public int Adet { get; set; }
        public decimal BirimFiyat { get; set; }

        // Navigation properties
        public Siparis Siparis { get; set; }
        public Urun Urun { get; set; }
    }
}
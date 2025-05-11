using B2BUygulamasi.Data;
using B2BUygulamasi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using B2BUygulamasi.Services;

namespace B2BUygulamasi.Controllers
{
    [Authorize]
    public class SepetController : Controller
    {
        private readonly B2BContext _context;
        private readonly ISepetService _sepetService;

        public SepetController(B2BContext context, ISepetService sepetService)
        {
            _context = context;
            _sepetService = sepetService;
        }

        // Sepeti Görüntüle
        public IActionResult Index()
        {
            var sepet = GetSepetFromSession();
            return View(sepet);
        }

        // Sepete Ekle (AJAX uyumlu)
        [HttpPost]
        [ValidateAntiForgeryToken] // CSRF koruması ekliyoruz
        public IActionResult Ekle(int urunId, int adet = 1)
        {
            try
            {
                // Kullanıcı giriş kontrolü (Authorize attribute zaten bunu sağlıyor)
                if (!User.Identity.IsAuthenticated)
                {
                    return Json(new { success = false, message = "Lütfen giriş yapınız!" });
                }

                // Ürün bilgilerini veritabanından al
                var urun = _context.Urunler
                    .Where(u => u.UrunID == urunId)
                    .Select(u => new { u.UrunAdi, u.Fiyat, u.StokMiktari })
                    .FirstOrDefault();

                if (urun == null)
                    return Json(new { success = false, message = "Ürün bulunamadı!" });

                if (urun.StokMiktari < adet)
                    return Json(new { success = false, message = "Yeterli stok yok!" });

                var sepet = GetSepetFromSession();
                var existingItem = sepet.FirstOrDefault(s => s.UrunId == urunId);

                if (existingItem != null)
                {
                    existingItem.Adet += adet;
                }
                else
                {
                    sepet.Add(new SepetItem
                    {
                        UrunId = urunId,
                        UrunAdi = urun.UrunAdi,
                        BirimFiyat = urun.Fiyat,
                        Adet = adet
                    });
                }

                SaveSepetToSession(sepet);

                return Json(new
                {
                    success = true,
                    sepetAdet = sepet.Sum(i => i.Adet),
                    message = $"{urun.UrunAdi} sepete eklendi!"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata oluştu: " + ex.Message });
            }
        }

        // Sepetten Sil (AJAX uyumlu)
        [HttpPost]
        public IActionResult Sil(int urunId)
        {
            var sepet = GetSepetFromSession();
            var item = sepet.FirstOrDefault(s => s.UrunId == urunId);

            if (item != null)
            {
                sepet.Remove(item);
                SaveSepetToSession(sepet);
                return Json(new
                {
                    success = true,
                    sepetAdet = sepet.Sum(i => i.Adet),
                    message = "Ürün sepetten kaldırıldı"
                });
            }

            return Json(new { success = false, message = "Ürün sepetinizde bulunamadı" });
        }

        // Sepet Özeti (Partial View için)
        public IActionResult SepetOzeti()
        {
            var sepet = GetSepetFromSession();
            return PartialView("_SepetOzeti", sepet);
        }

        // Siparişi Tamamla
        [HttpPost]
        public async Task<IActionResult> SiparisiTamamla()
        {
            var sepet = GetSepetFromSession();
            if (sepet == null || !sepet.Any())
            {
                TempData["Hata"] = "Sepetiniz boş!";
                return RedirectToAction(nameof(Index));
            }

            // Stok kontrolü
            foreach (var item in sepet)
            {
                var urun = await _context.Urunler.FindAsync(item.UrunId);
                if (urun == null || urun.StokMiktari < item.Adet)
                {
                    TempData["Hata"] = $"{item.UrunAdi} ürününden yeterli stok yok!";
                    return RedirectToAction(nameof(Index));
                }
            }

            // Sipariş oluştur
            var siparis = new Siparis
            {
                KullaniciId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                SiparisTarihi = DateTime.Now,
                ToplamTutar = sepet.Sum(item => item.BirimFiyat * item.Adet),
                Durum = "Hazırlanıyor"
            };

            _context.Siparisler.Add(siparis);
            await _context.SaveChangesAsync();

            // Sipariş detaylarını ekle
            foreach (var item in sepet)
            {
                _context.SiparisDetaylar.Add(new SiparisDetay
                {
                    SiparisId = siparis.SiparisId,
                    UrunId = item.UrunId,
                    Adet = item.Adet,
                    BirimFiyat = item.BirimFiyat
                });

                // Stok güncelle
                var urun = await _context.Urunler.FindAsync(item.UrunId);
                urun.StokMiktari -= item.Adet;
            }

            await _context.SaveChangesAsync();
            ClearSepet();

            TempData["Basarili"] = "Siparişiniz başarıyla oluşturuldu!";
            return RedirectToAction("Detay", "Siparis", new { id = siparis.SiparisId });
        }

        #region Yardımcı Metotlar
        private List<SepetItem> GetSepetFromSession()
        {
            var sessionData = HttpContext.Session.GetString("Sepet");
            return string.IsNullOrEmpty(sessionData)
                ? new List<SepetItem>()
                : JsonSerializer.Deserialize<List<SepetItem>>(sessionData);
        }

        private void SaveSepetToSession(List<SepetItem> sepet)
        {
            HttpContext.Session.SetString("Sepet", JsonSerializer.Serialize(sepet));
        }

        private void ClearSepet()
        {
            HttpContext.Session.Remove("Sepet");
        }
        #endregion
    }
}
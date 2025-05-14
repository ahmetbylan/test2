using B2BUygulamasi.Data;
using B2BUygulamasi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace B2BUygulamasi.Controllers
{
    [Authorize]
    public class SiparisController : Controller
    {
        private readonly B2BContext _context;

        public SiparisController(B2BContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Detay(int id)
        {
            var siparis = await _context.Siparisler
                .Include(s => s.SiparisDetaylar)
                    .ThenInclude(d => d.Urun)
                .FirstOrDefaultAsync(s => s.SiparisId == id);

            if (siparis == null)
                return NotFound();

            return View(siparis); // Görünüm: /Views/Siparis/Detay.cshtml
        }

        [HttpGet]
        public async Task<IActionResult> Index(string durum = null)
        {
            var kullaniciId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var query = _context.Siparisler
                .Include(s => s.SiparisDetaylar)
                .Where(s => s.KullaniciId == kullaniciId);

            if (!string.IsNullOrEmpty(durum))
            {
                query = query.Where(s => s.Durum == durum);
            }

            var siparisler = await query
                .OrderByDescending(s => s.SiparisTarihi)
                .ToListAsync();

            // Filtreleme seçenekleri ve seçili filtre
            ViewBag.DurumFiltre = new List<string> { "Hazırlanıyor", "Kargoda", "Tamamlandı" };
            ViewBag.SeciliDurum = string.IsNullOrEmpty(durum) ? "Tümü" : durum;

            return View(siparisler); // Görünüm: /Views/Siparis/Index.cshtml
        }
    }
}

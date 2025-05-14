using Microsoft.AspNetCore.Mvc;
using B2BUygulamasi.Filters;
using B2BUygulamasi.Models;
using B2BUygulamasi.Data;
using Microsoft.EntityFrameworkCore;

namespace B2BUygulamasi.Controllers
{
   // [KullaniciGirisFilter]
    public class HomeController : Controller
    {
        private readonly B2BContext _context;

        public HomeController(B2BContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IActionResult> Index()
        {
            // Null kontrolü ekledim
            if (!HttpContext.Session.TryGetValue("KullaniciID", out _))
            {
                return RedirectToAction("Login", "Account");
            }

            var kullaniciID = HttpContext.Session.GetInt32("KullaniciID");

            // KullanýcýID null kontrolü
            if (!kullaniciID.HasValue)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Account");
            }

            // Kullanýcýyý veritabanýndan çekerken null kontrolü
            var kullanici = await _context.Kullanicilar
                .Include(k => k.KullaniciRolleri)
                .ThenInclude(kr => kr.Rol)
                .AsNoTracking() // Performans için
                .FirstOrDefaultAsync(k => k.KullaniciID == kullaniciID);

            // Kullanýcý bulunamazsa
            if (kullanici == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Account");
            }

            // Modeli view'e gönder
            return View(kullanici);
        }
    }
}
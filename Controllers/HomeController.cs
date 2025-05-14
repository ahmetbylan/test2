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
            // Null kontrol� ekledim
            if (!HttpContext.Session.TryGetValue("KullaniciID", out _))
            {
                return RedirectToAction("Login", "Account");
            }

            var kullaniciID = HttpContext.Session.GetInt32("KullaniciID");

            // Kullan�c�ID null kontrol�
            if (!kullaniciID.HasValue)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Account");
            }

            // Kullan�c�y� veritaban�ndan �ekerken null kontrol�
            var kullanici = await _context.Kullanicilar
                .Include(k => k.KullaniciRolleri)
                .ThenInclude(kr => kr.Rol)
                .AsNoTracking() // Performans i�in
                .FirstOrDefaultAsync(k => k.KullaniciID == kullaniciID);

            // Kullan�c� bulunamazsa
            if (kullanici == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Account");
            }

            // Modeli view'e g�nder
            return View(kullanici);
        }
    }
}
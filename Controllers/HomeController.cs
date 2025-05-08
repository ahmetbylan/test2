using Microsoft.AspNetCore.Mvc;
using B2BUygulamasi.Filters;
using B2BUygulamasi.Models;
using B2BUygulamasi.Data;
using Microsoft.EntityFrameworkCore;

namespace B2BUygulamasi.Controllers
{
    [KullaniciGirisFilter]
    public class HomeController : Controller
    {
        private readonly B2BContext _context;

        public HomeController(B2BContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var kullaniciID = HttpContext.Session.GetInt32("KullaniciID");
            var kullanici = await _context.Kullanicilar
                .Include(k => k.KullaniciRolleri)
                .ThenInclude(kr => kr.Rol)
                .FirstOrDefaultAsync(k => k.KullaniciID == kullaniciID);

            return View(kullanici);
        }
    }
}
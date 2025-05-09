using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using B2BUygulamasi.Data;
using B2BUygulamasi.Models;
using System.Threading.Tasks;

namespace B2BUygulamasi.Controllers
{
    public class UrunController : Controller
    {
        private readonly B2BContext _context;

        public UrunController(B2BContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var urunler = await _context.Urunler.ToListAsync();
            return View(urunler);
        }
    }
}
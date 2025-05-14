using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using B2BUygulamasi.Data;
using B2BUygulamasi.Models;
using B2BUygulamasi.Services;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace B2BUygulamasi.Controllers
{
    public class UrunController : Controller
    {
        private readonly B2BContext _context;
        private readonly FiyatHesaplamaService _fiyatService;
        private readonly ILogger<UrunController> _logger;

        public UrunController(
            B2BContext context,
            FiyatHesaplamaService fiyatService,
            ILogger<UrunController> logger)
        {
            _context = context;
            _fiyatService = fiyatService;
            _logger = logger;
        }

        private async Task<Firma> GetCurrentFirmaAsync()
        {
            var kullaniciId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(kullaniciId))
            {
                return null;
            }

            return await _context.Kullanicilar
                .Include(k => k.Firma)
                .Where(k => k.KullaniciID == int.Parse(kullaniciId))
                .Select(k => k.Firma)
                .FirstOrDefaultAsync();
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var firma = await GetCurrentFirmaAsync();
                var toplamIskonto = firma != null ?
                    firma.BirinciIskontoOrani + firma.IkinciIskontoOrani : 0;

                var urunler = await _context.Urunler
                    .Where(u => u.AktifMi)
                    .OrderBy(u => u.Kategori)
                    .ThenBy(u => u.UrunAdi)
                    .ToListAsync();

                ViewBag.FirmaIskontoOrani = toplamIskonto;
                ViewBag.CurrentFirma = firma;

                return View(urunler);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ürün listesi alınırken hata oluştu");
                return View(new List<Urun>());
            }
        }
    }

    // ViewModel'ler controller içinde tanımlı (isterseniz ayrı dosyaya alabilirsiniz)
    public class UrunListeViewModelContainer
    {
        public double FirmaIskontoOrani { get; set; }
        public List<UrunListeViewModel> Urunler { get; set; } = new List<UrunListeViewModel>();
    }

    public class UrunListeViewModel
    {
        public int UrunID { get; set; }
        public string UrunAdi { get; set; }
        public string Kategori { get; set; }
        public int StokMiktari { get; set; }
        public decimal Fiyat { get; set; } // İskontolu fiyat
        public decimal OrijinalFiyat { get; set; }
        public string ResimYolu { get; set; }

        public string FiyatGosterim => Fiyat.ToString("C2");
        public string OrijinalFiyatGosterim => OrijinalFiyat.ToString("C2");
    }
}
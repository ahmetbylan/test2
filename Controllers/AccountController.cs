using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using B2BUygulamasi.Helpers;
using System.Diagnostics;
using B2BUygulamasi.Models;
using B2BUygulamasi.Data;

namespace B2BUygulamasi.Controllers
{
    public class AccountController : Controller
    {
        private readonly B2BContext _dbContext;
        private readonly ILogger<AccountController> _logger;

        public AccountController(B2BContext context, ILogger<AccountController> logger)
        {
            _dbContext = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var kullanici = await _dbContext.Kullanicilar
                    .AsNoTracking()
                    .FirstOrDefaultAsync(k => k.Email == model.Email);

                if (kullanici == null)
                {
                    _logger.LogWarning($"Kullanıcı bulunamadı: {model.Email}");
                    ModelState.AddModelError(string.Empty, "Geçersiz giriş denemesi");
                    return View(model);
                }

                if (!PasswordHasher.VerifyPassword("AQAAAAEAACcQAAAAEPdk5Hh3R4TZ4TzJ2lD5JYJ7vR6XfV7Lm8Kz1oW3X1o=", model.Password))
                {
                    _logger.LogWarning($"Hatalı şifre denemesi: {model.Email}");
                    ModelState.AddModelError(string.Empty, "Geçersiz giriş denemesi şifre");
                    return View(model);
                }

                if (!kullanici.Aktif)
                {
                    _logger.LogWarning($"Pasif hesap giriş denemesi: {kullanici.KullaniciID}");
                    ModelState.AddModelError(string.Empty, "Hesabınız aktif değil");
                    return View(model);
                }

                // Giriş başarılı
                kullanici.SonGirisTarihi = DateTime.Now;
                _dbContext.Update(kullanici);
                await _dbContext.SaveChangesAsync();

                HttpContext.Session.SetString("KullaniciEmail", kullanici.Email);
                HttpContext.Session.SetInt32("KullaniciID", kullanici.KullaniciID);

                _logger.LogInformation($"Başarılı giriş: {kullanici.KullaniciID}");
                return RedirectToLocal(returnUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Giriş işleminde hata oluştu");
                ModelState.AddModelError(string.Empty, "Giriş işlemi sırasında bir hata oluştu");
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                if (await _dbContext.Kullanicilar.AnyAsync(k => k.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Bu e-posta zaten kayıtlı");
                    return View(model);
                }

                var kullanici = new Kullanici
                {
                    Ad = model.Ad!.Trim(),
                    Soyad = model.Soyad!.Trim(),
                    Email = model.Email!.Trim().ToLower(),
                    Sifre = PasswordHasher.HashPassword(model.Sifre!),
                    FirmaAdi = model.FirmaAdi?.Trim(),
                    KayitTarihi = DateTime.Now,
                    Aktif = true
                };

                await _dbContext.Kullanicilar.AddAsync(kullanici);
                await _dbContext.SaveChangesAsync();

                // Varsayılan rol atama
                var standartRol = await _dbContext.Roller
                    .FirstOrDefaultAsync(r => r.RolAdi == "Standart");

                if (standartRol != null)
                {
                    await _dbContext.KullaniciRolleri.AddAsync(new KullaniciRol
                    {
                        KullaniciID = kullanici.KullaniciID,
                        RolID = standartRol.RolID
                    });
                    await _dbContext.SaveChangesAsync();
                }

                await transaction.CommitAsync();

                _logger.LogInformation($"Yeni kayıt: {kullanici.KullaniciID}");
                return RedirectToAction("Login", new { message = "Kayıt başarılı. Giriş yapabilirsiniz." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Kayıt işleminde hata oluştu");
                ModelState.AddModelError(string.Empty, "Kayıt işlemi sırasında bir hata oluştu");
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            try
            {
                var kullaniciId = HttpContext.Session.GetInt32("KullaniciID");
                HttpContext.Session.Clear();

                if (kullaniciId.HasValue)
                {
                    _logger.LogInformation($"Çıkış yapıldı: {kullaniciId}");
                }

                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Çıkış işleminde hata oluştu");
                return RedirectToAction("Index", "Home");
            }
        }

        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl!);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
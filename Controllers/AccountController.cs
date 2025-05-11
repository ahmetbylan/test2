using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using B2BUygulamasi.Helpers;
using System.Diagnostics;
using B2BUygulamasi.Models;
using B2BUygulamasi.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

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
                    .FirstOrDefaultAsync(k => k.Email == model.Email);

                if (kullanici == null)
                {
                    _logger.LogWarning($"Kullanıcı bulunamadı: {model.Email}");
                    ModelState.AddModelError(string.Empty, "Geçersiz giriş denemesi");
                    return View(model);
                }

                if (!PasswordHasher.VerifyPassword(model.Password, kullanici.Sifre))
                {
                    _logger.LogWarning("Şifre uyuşmuyor");
                    ModelState.AddModelError(string.Empty, "Geçersiz şifre");
                    return View(model);
                }

                if (!kullanici.Aktif)
                {
                    _logger.LogWarning($"Pasif hesap giriş denemesi: {kullanici.KullaniciID}");
                    ModelState.AddModelError(string.Empty, "Hesabınız aktif değil");
                    return View(model);
                }

                // Giriş başarılı: Son giriş güncelleme
                kullanici.SonGirisTarihi = DateTime.Now;
                _dbContext.Update(kullanici);
                await _dbContext.SaveChangesAsync();

                // Session
                HttpContext.Session.SetString("KullaniciEmail", kullanici.Email);
                HttpContext.Session.SetInt32("KullaniciID", kullanici.KullaniciID);

                // test çerez silincek 
                // Session oku ve logla
                var testEmail = HttpContext.Session.GetString("KullaniciEmail");
                _logger.LogWarning($"Test: Session'dan okunan email: {testEmail}");

                // Claims & Cookie Authentication
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, kullanici.KullaniciID.ToString()),
                    new Claim(ClaimTypes.Name, kullanici.Email),
                    new Claim(ClaimTypes.Email, kullanici.Email)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30)
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);

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
        public async Task<IActionResult> Logout()
        {
            try
            {
                HttpContext.Session.Clear();
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Çıkış yapılırken hata oluştu");
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

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using B2BUygulamasi.Helpers;
using B2BUygulamasi.Models;
using B2BUygulamasi.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

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
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var kullanici = await _dbContext.Kullanicilar
                    .Include(k => k.KullaniciRolleri)
                    .ThenInclude(kr => kr.Rol)
                    .FirstOrDefaultAsync(k => k.Email == model.Email);

                if (kullanici == null || !PasswordHasher.VerifyPassword(model.Password, kullanici.Sifre))
                {
                    ModelState.AddModelError(string.Empty, "Geçersiz email veya şifre");
                    return View(model);
                }

                if (!kullanici.Aktif)
                {
                    ModelState.AddModelError(string.Empty, "Hesabınız aktif değil");
                    return View(model);
                }

                // Son giriş tarihini güncelle
                kullanici.SonGirisTarihi = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();

                // Oturum (Session) bilgisi ekleniyor
                HttpContext.Session.SetInt32("KullaniciID", kullanici.KullaniciID);

                // Claim'leri oluştur
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, kullanici.KullaniciID.ToString()),
                    new Claim(ClaimTypes.Name, kullanici.Email),
                    new Claim(ClaimTypes.Email, kullanici.Email),
                    new Claim("LastLogin", DateTime.UtcNow.ToString("o"))
                };

                // Rolleri ekle
                foreach (var rol in kullanici.KullaniciRolleri.Select(kr => kr.Rol.RolAdi))
                {
                    claims.Add(new Claim(ClaimTypes.Role, rol));
                }

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30),
                    AllowRefresh = true
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)),
                    authProperties);

                _logger.LogInformation($"Kullanıcı giriş yaptı: {kullanici.Email}");
                return RedirectToLocal(returnUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Giriş hatası");
                ModelState.AddModelError(string.Empty, "Teknik bir hata oluştu");
                return View(model);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                if (await _dbContext.Kullanicilar.AnyAsync(k => k.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Bu email zaten kayıtlı");
                    return View(model);
                }

                var kullanici = new Kullanici
                {
                    Ad = model.Ad!.Trim(),
                    Soyad = model.Soyad!.Trim(),
                    Email = model.Email!.ToLower().Trim(),
                    Sifre = PasswordHasher.HashPassword(model.Sifre!),
                    FirmaAdi = model.FirmaAdi?.Trim(),
                    KayitTarihi = DateTime.UtcNow,
                    Aktif = true,
                    SonGirisTarihi = DateTime.UtcNow
                };

                // Varsayılan rol atama
                var standartRol = await _dbContext.Roller.FirstOrDefaultAsync(r => r.RolAdi == "Standart");
                if (standartRol != null)
                {
                    kullanici.KullaniciRolleri = new List<KullaniciRol>
                    {
                        new KullaniciRol { RolID = standartRol.RolID }
                    };
                }

                await _dbContext.Kullanicilar.AddAsync(kullanici);
                await _dbContext.SaveChangesAsync();

                // Oturum (Session) bilgisi ekleniyor
                HttpContext.Session.SetInt32("KullaniciID", kullanici.KullaniciID);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, kullanici.KullaniciID.ToString()),
                        new Claim(ClaimTypes.Name, kullanici.Email),
                        new Claim(ClaimTypes.Role, "Standart")
                    }, CookieAuthenticationDefaults.AuthenticationScheme)));

                _logger.LogInformation($"Yeni kayıt: {kullanici.Email}");
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kayıt hatası");
                ModelState.AddModelError(string.Empty, "Kayıt sırasında hata oluştu");
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Response.Cookies.Delete(".AspNetCore.Cookies");
            Response.Cookies.Delete(".AspNetCore.Session");

            TempData["CikisMesaji"] = "Başarıyla çıkış yaptınız.";
            _logger.LogInformation("Kullanıcı çıkış yaptı");

            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        [Authorize]
        public IActionResult Profile()
        {
            return View();
        }

        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }
    }
}

using B2BUygulamasi.Services;
using Microsoft.EntityFrameworkCore;
using B2BUygulamasi.Data;
using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.Cookies;
using B2BUygulamasi.Controllers;
using B2BUygulamasi.Components; // ViewComponent'ler için eklendi

var builder = WebApplication.CreateBuilder(args);

// 1. Configuration
var configuration = builder.Configuration;

// 2. Services Configuration
builder.Services.AddDbContext<B2BContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("B2BConnection"),
    sqlOptions => sqlOptions.EnableRetryOnFailure()));

// Identity Configuration
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
})
.AddEntityFrameworkStores<B2BContext>()
.AddDefaultTokenProviders();

// Session Configuration
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = builder.Environment.IsDevelopment() ? CookieSecurePolicy.None : CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = "B2B.Session";
});

// MVC Configuration
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

// Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
    });

// Application Services
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ISepetService, SepetService>();
builder.Services.AddScoped<ErrorController>();
builder.Services.AddTransient<SepetSummaryViewComponent>(); // ViewComponent servis kaydý eklendi

var app = builder.Build();

// 3. Middleware Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    // Database Migration
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<B2BContext>();
        try
        {
            dbContext.Database.Migrate();
            // Seed Data ekleyebilirsiniz
            // DbInitializer.Seed(dbContext);
        }
        catch (Exception ex)
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "Veritabaný migration hatasý");
        }
    }
}
else
{
    app.UseExceptionHandler("/Error/ServerError");
    app.UseStatusCodePagesWithReExecute("/Error/{0}");
    app.UseHsts();
}

// Security Headers
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "SAMEORIGIN");
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
    await next();
});

app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=31536000");
    }
});

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

// Localization (Türkçe için)
var supportedCultures = new[] { "tr-TR" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);

// Endpoints
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// ViewComponent'ler için gerekli endpoint
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapFallbackToController("NotFound", "Error");
});

app.Run();
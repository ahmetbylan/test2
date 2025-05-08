using Microsoft.EntityFrameworkCore;
using B2BUygulamasi.Models;
using B2BUygulamasi.Data;
using Microsoft.AspNetCore.Session;

var builder = WebApplication.CreateBuilder(args);

// Veritaban� ba�lant�s�n� ekle
builder.Services.AddDbContext<B2BContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("B2BConnection")));
// Di�er servisler...
builder.Services.AddControllersWithViews();
// Session servisini ekleyin
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout s�resi
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();



// Veritaban�n� otomatik migrate et (development ortam�nda)
//if (app.Environment.IsDevelopment())
//{
//    using (var scope = app.Services.CreateScope())
//    {
//        var dbContext = scope.ServiceProvider.GetRequiredService<B2BContext>();
//        dbContext.Database.Migrate();
//    }
//}

// Di�er middleware'ler...
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
// Session middleware'ini ekleyin (UseRouting'den sonra)
app.UseSession(); // Bu sat�r� ekleyin
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
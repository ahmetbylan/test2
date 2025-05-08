using Microsoft.EntityFrameworkCore;
using B2BUygulamasi.Models;
using B2BUygulamasi.Data;

var builder = WebApplication.CreateBuilder(args);

// Veritabaný baðlantýsýný ekle
builder.Services.AddDbContext<B2BContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("B2BConnection")));
// Diðer servisler...
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Veritabanýný otomatik migrate et (development ortamýnda)
//if (app.Environment.IsDevelopment())
//{
//    using (var scope = app.Services.CreateScope())
//    {
//        var dbContext = scope.ServiceProvider.GetRequiredService<B2BContext>();
//        dbContext.Database.Migrate();
//    }
//}

// Diðer middleware'ler...
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
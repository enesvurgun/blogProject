using AspNetCoreHero.ToastNotification; // ToastNotification kütüphanesi, bildirimler için.
using AspNetCoreHero.ToastNotification.Extensions; // ToastNotification kütüphanesi uzantýlarý.
using projeBlog.Data; // Veritabaný baðlamý için.
using projeBlog.Models; // Model sýnýflarý için.
using projeBlog.Utilites; // Yardýmcý sýnýflar için.
using Microsoft.AspNetCore.Identity; // ASP.NET Core Identity kütüphanesi.
using Microsoft.EntityFrameworkCore; // Entity Framework Core kütüphanesi.

var builder = WebApplication.CreateBuilder(args); // Web uygulamasý oluþturucu.

builder.Services.AddControllersWithViews(); // MVC için gerekli servisleri ekler.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection"); // Veritabaný baðlantý dizesini alýr.

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString)); // Veritabaný baðlamýný ve SQL Server baðlantýsýný yapýlandýrýr.

builder.Services.AddIdentity<ApplicationUser, IdentityRole>() // Identity servisini ekler ve yapýlandýrýr.
    .AddEntityFrameworkStores<ApplicationDbContext>() // Entity Framework maðazalarýný kullanacak þekilde yapýlandýrýr.
    .AddDefaultTokenProviders(); // Varsayýlan token saðlayýcýlarýný ekler.

builder.Services.AddScoped<IDbInitializer, DbInitializer>(); // IDbInitializer servisini DbInitializer ile iliþkilendirir.

builder.Services.AddNotyf(config => { config.DurationInSeconds = 10; config.IsDismissable = true; config.Position = NotyfPosition.BottomRight; }); // ToastNotification servisini yapýlandýrýr.

builder.Services.ConfigureApplicationCookie(options => // Uygulama çerezlerini yapýlandýrýr.
{
    options.LoginPath = "/login"; // Giriþ sayfasý yolu.
    options.AccessDeniedPath = "/AccessDenied"; // Eriþim reddedildi sayfasý yolu.
});

var app = builder.Build(); // Web uygulamasýný oluþturur.
DataSeeding(); // Veritabaný baþlangýç verilerini ekler.

if (!app.Environment.IsDevelopment()) // Geliþtirme ortamýnda deðilse
{
    app.UseExceptionHandler("/Home/Error"); // Hata sayfasýný kullan.
    app.UseHsts(); // HSTS'yi etkinleþtir.
}

app.UseNotyf(); // ToastNotification servisini kullanýr.

app.UseHttpsRedirection(); // HTTPS yönlendirmeyi etkinleþtirir.
app.UseStaticFiles(); // Statik dosya sunumunu etkinleþtirir.

app.UseRouting(); // Yönlendirmeyi etkinleþtirir.

app.UseAuthentication(); // Kimlik doðrulamayý etkinleþtirir.

app.UseAuthorization(); // Yetkilendirmeyi etkinleþtirir.

app.MapControllerRoute( // Alan (area) yönlendirme kuralýný belirler.
    name: "area",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute( // Varsayýlan yönlendirme kuralýný belirler.
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run(); // Uygulamayý çalýþtýrýr.

void DataSeeding() // Veritabaný baþlangýç verilerini ekler.
{
    using (var scope = app.Services.CreateScope()) // Servis kapsamý oluþturur.
    {
        var DbInitialize = scope.ServiceProvider.GetRequiredService<IDbInitializer>(); // IDbInitializer servisini alýr.
        DbInitialize.Initialize(); // Veritabaný baþlangýç verilerini ekler.
    }
}

using AspNetCoreHero.ToastNotification; // ToastNotification k�t�phanesi, bildirimler i�in.
using AspNetCoreHero.ToastNotification.Extensions; // ToastNotification k�t�phanesi uzant�lar�.
using projeBlog.Data; // Veritaban� ba�lam� i�in.
using projeBlog.Models; // Model s�n�flar� i�in.
using projeBlog.Utilites; // Yard�mc� s�n�flar i�in.
using Microsoft.AspNetCore.Identity; // ASP.NET Core Identity k�t�phanesi.
using Microsoft.EntityFrameworkCore; // Entity Framework Core k�t�phanesi.

var builder = WebApplication.CreateBuilder(args); // Web uygulamas� olu�turucu.

builder.Services.AddControllersWithViews(); // MVC i�in gerekli servisleri ekler.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection"); // Veritaban� ba�lant� dizesini al�r.

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString)); // Veritaban� ba�lam�n� ve SQL Server ba�lant�s�n� yap�land�r�r.

builder.Services.AddIdentity<ApplicationUser, IdentityRole>() // Identity servisini ekler ve yap�land�r�r.
    .AddEntityFrameworkStores<ApplicationDbContext>() // Entity Framework ma�azalar�n� kullanacak �ekilde yap�land�r�r.
    .AddDefaultTokenProviders(); // Varsay�lan token sa�lay�c�lar�n� ekler.

builder.Services.AddScoped<IDbInitializer, DbInitializer>(); // IDbInitializer servisini DbInitializer ile ili�kilendirir.

builder.Services.AddNotyf(config => { config.DurationInSeconds = 10; config.IsDismissable = true; config.Position = NotyfPosition.BottomRight; }); // ToastNotification servisini yap�land�r�r.

builder.Services.ConfigureApplicationCookie(options => // Uygulama �erezlerini yap�land�r�r.
{
    options.LoginPath = "/login"; // Giri� sayfas� yolu.
    options.AccessDeniedPath = "/AccessDenied"; // Eri�im reddedildi sayfas� yolu.
});

var app = builder.Build(); // Web uygulamas�n� olu�turur.
DataSeeding(); // Veritaban� ba�lang�� verilerini ekler.

if (!app.Environment.IsDevelopment()) // Geli�tirme ortam�nda de�ilse
{
    app.UseExceptionHandler("/Home/Error"); // Hata sayfas�n� kullan.
    app.UseHsts(); // HSTS'yi etkinle�tir.
}

app.UseNotyf(); // ToastNotification servisini kullan�r.

app.UseHttpsRedirection(); // HTTPS y�nlendirmeyi etkinle�tirir.
app.UseStaticFiles(); // Statik dosya sunumunu etkinle�tirir.

app.UseRouting(); // Y�nlendirmeyi etkinle�tirir.

app.UseAuthentication(); // Kimlik do�rulamay� etkinle�tirir.

app.UseAuthorization(); // Yetkilendirmeyi etkinle�tirir.

app.MapControllerRoute( // Alan (area) y�nlendirme kural�n� belirler.
    name: "area",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute( // Varsay�lan y�nlendirme kural�n� belirler.
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run(); // Uygulamay� �al��t�r�r.

void DataSeeding() // Veritaban� ba�lang�� verilerini ekler.
{
    using (var scope = app.Services.CreateScope()) // Servis kapsam� olu�turur.
    {
        var DbInitialize = scope.ServiceProvider.GetRequiredService<IDbInitializer>(); // IDbInitializer servisini al�r.
        DbInitialize.Initialize(); // Veritaban� ba�lang�� verilerini ekler.
    }
}

using AspNetCoreHero.ToastNotification.Abstractions; // Bildirim servisi için gerekli kütüphane
using projeBlog.Data; // Veri tabanı bağlamı
using projeBlog.Models; // Modeller
using projeBlog.ViewModels; // Görüntü Modelleri (View Models)
using Microsoft.AspNetCore.Authorization; // Yetkilendirme
using Microsoft.AspNetCore.Identity; // Kimlik yönetimi
using Microsoft.AspNetCore.Mvc; // MVC için gerekli
using Microsoft.EntityFrameworkCore; // Entity Framework Core için gerekli
using Microsoft.Extensions.Hosting; // Barındırma ortamı için gerekli

namespace projeBlog.Areas.Admin.Controllers
{
    [Area("Admin")] // Bu denetleyicinin "Admin" alanına ait olduğunu belirtir
    [Authorize(Roles = "Admin")] // Bu denetleyiciye sadece "Admin" rolüne sahip kullanıcılar erişebilir
    public class PageController : Controller
    {
        private readonly ApplicationDbContext _context; // Veri tabanı bağlamı
        public INotyfService _notification { get; } // Bildirim servisi
        private readonly IWebHostEnvironment _webHostEnvironment; // Web barındırma ortamı

        // Yapıcı metod (Constructor)
        public PageController(ApplicationDbContext context,
                              INotyfService notification,
                              IWebHostEnvironment webHostEnvironment)
        {
            _context = context; // Veri tabanı bağlamını ayarlar
            _notification = notification; // Bildirim servisini ayarlar
            _webHostEnvironment = webHostEnvironment; // Web barındırma ortamını ayarlar
        }

        // Hakkımda sayfasını görüntülemek için HTTP GET isteği
        [HttpGet]
        public async Task<IActionResult> About()
        {
            // "about" slugına sahip sayfayı veri tabanından getirir
            var page = await _context.Pages!.FirstOrDefaultAsync(x => x.Slug == "about");

            // Sayfa verilerini ViewModel'e aktarır
            var vm = new PageVM()
            {
                Id = page!.Id,
                Title = page.Title,
                ShortDescription = page.ShortDescription,
                Description = page.Description,
                ThumbnailUrl = page.ThumbnailUrl,
            };

            // View'e ViewModel'i gönderir
            return View(vm);
        }

        // Hakkımda sayfasını güncellemek için HTTP POST isteği
        [HttpPost]
        public async Task<IActionResult> About(PageVM vm)
        {
            // Model doğrulama başarısız olursa, aynı View'i geri döndür
            if (!ModelState.IsValid) { return View(vm); }

            // "about" slugına sahip sayfayı veri tabanından getirir
            var page = await _context.Pages!.FirstOrDefaultAsync(x => x.Slug == "about");
            if (page == null)
            {
                _notification.Error("Sayfa bulunamadı."); // Sayfa bulunamazsa hata bildirimi gösterir
                return View();
            }

            // Sayfa verilerini günceller
            page.Title = vm.Title;
            page.ShortDescription = vm.ShortDescription;
            page.Description = vm.Description;

            // Yeni bir thumbnail yüklenmişse, thumbnail URL'sini günceller
            if (vm.Thumbnail != null)
            {
                page.ThumbnailUrl = UploadImage(vm.Thumbnail);
            }

            // Değişiklikleri veri tabanına kaydeder
            await _context.SaveChangesAsync();
            _notification.Success("Hakkımda sayfası başarılı bir şekilde güncellendi."); // Başarı bildirimi gösterir
            return RedirectToAction("About", "Page", new { area = "Admin" }); // "About" sayfasına yönlendirir
        }

        // İletişim sayfasını görüntülemek için HTTP GET isteği
        [HttpGet]
        public async Task<IActionResult> Contact()
        {
            // "contact" slugına sahip sayfayı veri tabanından getirir
            var page = await _context.Pages!.FirstOrDefaultAsync(x => x.Slug == "contact");

            // Sayfa verilerini ViewModel'e aktarır
            var vm = new PageVM()
            {
                Id = page!.Id,
                Title = page.Title,
                ShortDescription = page.ShortDescription,
                Description = page.Description,
                ThumbnailUrl = page.ThumbnailUrl,
            };

            // View'e ViewModel'i gönderir
            return View(vm);
        }

        // İletişim sayfasını güncellemek için HTTP POST isteği
        [HttpPost]
        public async Task<IActionResult> Contact(PageVM vm)
        {
            // Model doğrulama başarısız olursa, aynı View'i geri döndür
            if (!ModelState.IsValid) { return View(vm); }

            // "contact" slugına sahip sayfayı veri tabanından getirir
            var page = await _context.Pages!.FirstOrDefaultAsync(x => x.Slug == "contact");
            if (page == null)
            {
                _notification.Error("Sayfa bulunamadı."); // Sayfa bulunamazsa hata bildirimi gösterir
                return View();
            }

            // Sayfa verilerini günceller
            page.Title = vm.Title;
            page.ShortDescription = vm.ShortDescription;
            page.Description = vm.Description;

            // Yeni bir thumbnail yüklenmişse, thumbnail URL'sini günceller
            if (vm.Thumbnail != null)
            {
                page.ThumbnailUrl = UploadImage(vm.Thumbnail);
            }

            // Değişiklikleri veri tabanına kaydeder
            await _context.SaveChangesAsync();
            _notification.Success("İletişim sayfası başarılı bir şekilde güncellendi."); // Başarı bildirimi gösterir
            return RedirectToAction("Contact", "Page", new { area = "Admin" }); // "Contact" sayfasına yönlendirir
        }

        // Resim yükleme işlemini gerçekleştiren metod
        private string UploadImage(IFormFile file)
        {
            string uniqueFileName = ""; // Benzersiz dosya adı oluşturur
            var folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "thumbnails"); // Dosyanın yükleneceği klasör yolu
            uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName; // Benzersiz dosya adı oluşturur
            var filePath = Path.Combine(folderPath, uniqueFileName); // Dosyanın tam yolunu oluşturur
            using (FileStream fileStream = System.IO.File.Create(filePath)) // Dosya akışı oluşturur
            {
                file.CopyTo(fileStream); // Dosyayı akışa kopyalar
            }
            return uniqueFileName; // Benzersiz dosya adını döndürür
        }
    }
}

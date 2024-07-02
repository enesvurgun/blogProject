using AspNetCoreHero.ToastNotification.Abstractions; // Bildirim servisi için gerekli kütüphane
using projeBlog.Data; // Veri tabanı bağlamı
using projeBlog.Models; // Modeller
using projeBlog.ViewModels; // Görüntü Modelleri (View Models)
using Microsoft.AspNetCore.Authorization; // Yetkilendirme
using Microsoft.AspNetCore.Mvc; // MVC için gerekli
using Microsoft.EntityFrameworkCore; // Entity Framework Core için gerekli
using Microsoft.Extensions.Hosting; // Barındırma ortamı için gerekli

namespace projeBlog.Areas.Admin.Controllers
{
    [Area("Admin")] // Bu denetleyicinin "Admin" alanına ait olduğunu belirtir
    [Authorize(Roles = "Admin")] // Bu denetleyiciye sadece "Admin" rolüne sahip kullanıcılar erişebilir
    public class SettingController : Controller
    {
        private readonly ApplicationDbContext _context; // Veri tabanı bağlamı
        public INotyfService _notification { get; } // Bildirim servisi
        private readonly IWebHostEnvironment _webHostEnvironment; // Web barındırma ortamı

        // Yapıcı metod (Constructor)
        public SettingController(ApplicationDbContext context,
                                INotyfService notyfService,
                                IWebHostEnvironment webHostEnvironment)
        {
            _context = context; // Veri tabanı bağlamını ayarlar
            _notification = notyfService; // Bildirim servisini ayarlar
            _webHostEnvironment = webHostEnvironment; // Web barındırma ortamını ayarlar
        }

        // Ayarlar sayfasını görüntülemek için HTTP GET isteği
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var settings = _context.Settings!.ToList(); // Ayarları veri tabanından alır
            if (settings.Count > 0)
            {
                // Eğer ayarlar varsa, bunları ViewModel'e aktarır
                var vm = new SettingVM()
                {
                    Id = settings[0].Id,
                    SiteName = settings[0].SiteName,
                    Title = settings[0].Title,
                    ShortDescription = settings[0].ShortDescription,
                    ThumbnailUrl = settings[0].ThumbnailUrl,
                };
                return View(vm); // View'e ViewModel'i gönderir
            }
            // Eğer ayarlar yoksa, varsayılan bir ayar oluşturur
            var setting = new Setting()
            {
                SiteName = "Demo Name",
            };
            await _context.Settings!.AddAsync(setting); // Yeni ayarı veri tabanına ekler
            await _context.SaveChangesAsync(); // Değişiklikleri kaydeder
            var createdSettings = _context.Settings!.ToList(); // Eklenen ayarları alır
            var createdVm = new SettingVM()
            {
                Id = createdSettings[0].Id,
                SiteName = createdSettings[0].SiteName,
                Title = createdSettings[0].Title,
                ShortDescription = createdSettings[0].ShortDescription,
                ThumbnailUrl = createdSettings[0].ThumbnailUrl,
            };
            return View(createdVm); // Yeni oluşturulan ayarları ViewModel ile döndürür
        }

        // Ayarları güncellemek için HTTP POST isteği
        [HttpPost]
        public async Task<IActionResult> Index(SettingVM vm)
        {
            if (!ModelState.IsValid) { return View(vm); } // Model doğrulama başarısız olursa, aynı View'i geri döndürür
            var setting = await _context.Settings!.FirstOrDefaultAsync(x => x.Id == vm.Id); // Güncellenecek ayarı bulur
            if (setting == null)
            {
                _notification.Error("Bir şeyler ters gitti."); // Ayar bulunamazsa hata bildirimi gösterir
                return View(vm);
            }
            // Ayar verilerini günceller
            setting.SiteName = vm.SiteName;
            setting.Title = vm.Title;
            setting.ShortDescription = vm.ShortDescription;

            // Yeni bir thumbnail yüklenmişse, thumbnail URL'sini günceller
            if (vm.Thumbnail != null)
            {
                setting.ThumbnailUrl = UploadImage(vm.Thumbnail);
            }
            await _context.SaveChangesAsync(); // Değişiklikleri veri tabanına kaydeder
            _notification.Success("Ayarlar başarıyla güncellendi."); // Başarı bildirimi gösterir
            return RedirectToAction("Index", "Setting", new { area = "Admin" }); // Ayarlar sayfasına yönlendirir
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

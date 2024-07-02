using projeBlog.Data; // Veritabanı erişim katmanı
using projeBlog.Models; // Model sınıfları
using projeBlog.ViewModels; // ViewModel sınıfları
using Microsoft.AspNetCore.Mvc; // ASP.NET Core MVC kütüphanesi
using Microsoft.EntityFrameworkCore; // Entity Framework Core kütüphanesi
using System.Diagnostics; // Debugging ve performans izleme kütüphanesi
using X.PagedList; // Sayfalama işlemleri için kütüphane

namespace projeBlog.Controllers // projeBlog.Controllers ad alanı
{
    public class HomeController : Controller // HomeController sınıfı, Controller sınıfından türetilir
    {
        private readonly ILogger<HomeController> _logger; // ILogger türünde bir alan. Loglama işlemleri için kullanılır.
        private readonly ApplicationDbContext _context; // ApplicationDbContext türünde bir alan. Veritabanı erişimi için kullanılır.

        public HomeController(ILogger<HomeController> logger,
                                ApplicationDbContext context) // HomeController sınıfının yapıcı metodu
        {
            _logger = logger; // Loglama hizmetini atar.
            _context = context; // Veritabanı bağlamını atar.
        }

        public async Task<IActionResult> Index(int? page) // Index metodu, HTTP GET isteklerini karşılar ve sayfalama parametresi alır.
        {
            var vm = new HomeVM(); // HomeVM türünde yeni bir ViewModel nesnesi oluşturur.
            var setting = _context.Settings!.ToList(); // Veritabanındaki ayarları alır.
            vm.Title = setting[0].Title; // ViewModel'in başlık alanını ayarlamak için ayarlardan alınan değeri kullanır.
            vm.ShortDescription = setting[0].ShortDescription; // ViewModel'in kısa açıklama alanını ayarlamak için ayarlardan alınan değeri kullanır.
            vm.ThumbnailUrl = setting[0].ThumbnailUrl; // ViewModel'in küçük resim URL'sini ayarlamak için ayarlardan alınan değeri kullanır.
            int pageSize = 4; // Her sayfada gösterilecek gönderi sayısını belirler.
            int pageNumber = (page ?? 1); // Sayfa numarasını belirler. Eğer "page" parametresi null ise 1 olarak ayarlar.
            vm.Posts = await _context.Posts!.Include(x => x.ApplicationUser).OrderByDescending(x => x.CreatedDate).ToPagedListAsync(pageNumber, pageSize); // Gönderileri veritabanından alır ve sayfalar.
            return View(vm); // View'e HomeVM nesnesini model olarak gönderir.
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)] // Bu metot için önbelleğe alma ayarlarını belirler.
        public IActionResult Error() // Hata durumlarında çağrılan metot.
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }); // Hata görünümünü döner ve hata modeli ile birlikte döner.
        }
    }
}

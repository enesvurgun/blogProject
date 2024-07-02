using AspNetCoreHero.ToastNotification.Abstractions; // ToastNotification kütüphanesi, bildirimleri göstermek için kullanılır.
using projeBlog.Data; // projeBlog projesinin veri erişim katmanını kullanır.
using projeBlog.ViewModels; // projeBlog projesinin ViewModel'lerini kullanır.
using Microsoft.AspNetCore.Mvc; // ASP.NET Core MVC kütüphanesini kullanır.
using Microsoft.EntityFrameworkCore; // Entity Framework Core kütüphanesini kullanır.

namespace projeBlog.Controllers // projeBlog.Controllers ad alanı
{
    public class BlogController : Controller // BlogController sınıfı, Controller sınıfından türetilir.
    {
        private readonly ApplicationDbContext _context; // ApplicationDbContext türünde bir alan. Veritabanı erişimi için kullanılır.
        public INotyfService _notification { get; } // INotyfService türünde bir alan. Bildirimleri göstermek için kullanılır.

        public BlogController(ApplicationDbContext context, INotyfService notification) // BlogController sınıfının yapıcı metodu
        {
            _context = context; // Veritabanı bağlamını atar.
            _notification = notification; // Bildirim hizmetini atar.
        }

        [HttpGet("[controller]/{slug}")] // HTTP GET isteklerini karşılar.
        public IActionResult Post(string slug) // Post metodu, "slug" parametresini alır.
        {
            if (slug == "") // Eğer "slug" boş ise
            {
                _notification.Error("Gönderi bulunamadı."); // Hata bildirimi gösterir.
                return View(); // Boş bir görünüm döner.
            }
            var post = _context.Posts!.Include(x => x.ApplicationUser).FirstOrDefault(x => x.Slug == slug); // Veritabanında "slug" ile eşleşen gönderiyi bulur.
            if (post == null) // Eğer gönderi bulunamazsa
            {
                _notification.Error("Gönderi bulunamadı."); // Hata bildirimi gösterir.
                return View(); // Boş bir görünüm döner.
            }
            var vm = new BlogPostVM() // BlogPostVM türünde yeni bir nesne oluşturur.
            {
                Id = post.Id, // Gönderinin ID'sini atar.
                Title = post.Title, // Gönderinin başlığını atar.
                AuthorName = post.ApplicationUser!.FirstName + " " + post.ApplicationUser.LastName, // Yazarın adını ve soyadını atar.
                CreatedDate = post.CreatedDate, // Gönderinin oluşturulma tarihini atar.
                ThumbnailUrl = post.ThumbnailUrl, // Gönderinin küçük resim URL'sini atar.
                Description = post.Description, // Gönderinin açıklamasını atar.
                ShortDescription = post.ShortDescription, // Gönderinin kısa açıklamasını atar.
            };
            return View(vm); // View'e BlogPostVM nesnesini model olarak gönderir.
        }
    }
}

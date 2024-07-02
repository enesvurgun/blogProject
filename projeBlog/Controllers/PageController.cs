using projeBlog.Data; // projeBlog projesinin veri erişim katmanını kullanır.
using projeBlog.ViewModels; // projeBlog projesinin ViewModel'lerini kullanır.
using Microsoft.AspNetCore.Mvc; // ASP.NET Core MVC kütüphanesini kullanır.
using Microsoft.EntityFrameworkCore; // Entity Framework Core kütüphanesini kullanır.
using projeBlog.Models; // projeBlog projesinin model sınıflarını kullanır.
using AspNetCoreHero.ToastNotification.Abstractions; // ToastNotification kütüphanesini kullanır, bildirimler için.
using projeBlog.Utilites; // projeBlog projesinin yardımcı (utility) sınıflarını kullanır.

namespace projeBlog.Controllers // projeBlog.Controllers ad alanı
{
    public class PageController : Controller // PageController sınıfı, Controller sınıfından türetilir
    {
        private readonly ApplicationDbContext _context; // ApplicationDbContext türünde bir alan. Veritabanı erişimi için kullanılır.
        public INotyfService _notification { get; } // INotyfService türünde bir alan. Bildirimleri göstermek için kullanılır.

        public PageController(ApplicationDbContext context, INotyfService notyfService) // PageController sınıfının yapıcı metodu
        {
            _context = context; // Veritabanı bağlamını atar.
            _notification = notyfService; // Bildirim hizmetini atar.
        }

        public async Task<IActionResult> About() // Hakkında sayfası için HTTP GET metodu
        {
            var page = await _context.Pages!.FirstOrDefaultAsync(x => x.Slug == "about"); // Veritabanında "about" slug'ına sahip sayfayı bulur.
            var vm = new PageVM() // PageVM türünde yeni bir ViewModel nesnesi oluşturur.
            {
                Title = page!.Title, // Sayfa başlığını ViewModel'e atar.
                ShortDescription = page.ShortDescription, // Kısa açıklamayı ViewModel'e atar.
                Description = page.Description, // Açıklamayı ViewModel'e atar.
                ThumbnailUrl = page.ThumbnailUrl, // Küçük resim URL'sini ViewModel'e atar.
            };
            return View(vm); // View'e PageVM nesnesini model olarak gönderir.
        }

        [HttpGet]
        public async Task<IActionResult> Contact() // İletişim sayfası için HTTP GET metodu
        {
            var page = await _context.Pages!.FirstOrDefaultAsync(x => x.Slug == "contact"); // Veritabanında "contact" slug'ına sahip sayfayı bulur.

            var vm = new ContactVM(); // ContactVM türünde yeni bir ViewModel nesnesi oluşturur.
            vm.PageVM.Title = page!.Title; // Sayfa başlığını ViewModel'e atar.
            vm.PageVM.ShortDescription = page.ShortDescription; // Kısa açıklamayı ViewModel'e atar.
            vm.PageVM.Description = page.Description; // Açıklamayı ViewModel'e atar.
            vm.PageVM.ThumbnailUrl = page.ThumbnailUrl; // Küçük resim URL'sini ViewModel'e atar.

            return View(vm); // View'e ContactVM nesnesini model olarak gönderir.
        }

        [HttpPost]
        public async Task<IActionResult> Contact(ContactVM contactVM) // İletişim formu için HTTP POST metodu
        {
            if (!ModelState.IsValid) { return View(contactVM); } // Model durumu geçerli değilse, formu aynı model ile yeniden gösterir.

            Contact contact = new Contact(); // Yeni bir Contact nesnesi oluşturur.
            contact.Name = contactVM.Name; // İletişim formundan adı alır ve atar.
            contact.Email = contactVM.Email; // İletişim formundan e-posta adresini alır ve atar.
            contact.Subject = contactVM.Subject; // İletişim formundan konuyu alır ve atar.
            contact.Message = contactVM.Message; // İletişim formundan mesajı alır ve atar.

            await _context.AddAsync(contact); // Yeni iletişim kaydını veritabanına ekler.
            await _context.SaveChangesAsync(); // Değişiklikleri veritabanına kaydeder.

            await MailSender.Send(contact.Email, contact.Subject, contact.Message); // Mesajı belirtilen e-posta adresine gönderir.
            _notification.Success("Mesajınız başarılı bir şekilde gönderildi."); // Başarılı bir gönderim bildirimi gösterir.
            return View(new ContactVM()); // Yeni bir ContactVM nesnesi ile formu yeniden gösterir.
        }
    }
}

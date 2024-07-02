using projeBlog.Data; // projeBlog projesinin veri erişim katmanını kullanır.
using projeBlog.Models; // projeBlog projesinin model sınıflarını kullanır.
using Microsoft.AspNetCore.Identity; // ASP.NET Core Identity kütüphanesini kullanır.

namespace projeBlog.Utilites // projeBlog.Utilites ad alanı
{
    public class DbInitializer : IDbInitializer // DbInitializer sınıfı, IDbInitializer arayüzünü uygular
    {
        private readonly ApplicationDbContext _context; // ApplicationDbContext türünde bir alan. Veritabanı erişimi için kullanılır.
        private readonly UserManager<ApplicationUser> _userManager; // UserManager türünde bir alan. Kullanıcı yönetimi için kullanılır.
        private readonly RoleManager<IdentityRole> _roleManager; // RoleManager türünde bir alan. Rol yönetimi için kullanılır.

        public DbInitializer(ApplicationDbContext context,
                             UserManager<ApplicationUser> userManager,
                             RoleManager<IdentityRole> roleManager) // DbInitializer sınıfının yapıcı metodu
        {
            _context = context; // Veritabanı bağlamını atar.
            _userManager = userManager; // Kullanıcı yönetimi hizmetini atar.
            _roleManager = roleManager; // Rol yönetimi hizmetini atar.
        }

        public void Initialize() // Veritabanını başlatan metot
        {
            // WebsiteAdmin rolü mevcut değilse, roller ve admin kullanıcısı oluşturulur
            if (!_roleManager.RoleExistsAsync(WebsiteRoles.WebsiteAdmin).GetAwaiter().GetResult())
            {
                // WebsiteAdmin rolü oluşturulur
                _roleManager.CreateAsync(new IdentityRole(WebsiteRoles.WebsiteAdmin)).GetAwaiter().GetResult();
                // WebsiteAuthor rolü oluşturulur
                _roleManager.CreateAsync(new IdentityRole(WebsiteRoles.WebsiteAuthor)).GetAwaiter().GetResult();
                // Admin kullanıcısı oluşturulur
                _userManager.CreateAsync(new ApplicationUser()
                {
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    FirstName = "Super",
                    LastName = "Admin"
                }, "Admin@0011").Wait(); // Şifre ile birlikte

                // Admin kullanıcısı veritabanından alınır
                var appUser = _context.ApplicationUsers!.FirstOrDefault(x => x.Email == "admin@gmail.com");
                if (appUser != null) // Eğer admin kullanıcısı bulunursa
                {
                    // Admin kullanıcısına WebsiteAdmin rolü atanır
                    _userManager.AddToRoleAsync(appUser, WebsiteRoles.WebsiteAdmin).GetAwaiter().GetResult();
                }

                // Varsayılan sayfalar oluşturulur
                var listOfPages = new List<Page>()
                {
                    new Page()
                    {
                        Title = "About Us",
                        Slug = "about"
                    },
                    new Page()
                    {
                        Title = "Contact Us",
                        Slug = "contact"
                    }
                };

                // Sayfalar veritabanına eklenir
                _context.Pages!.AddRange(listOfPages);

                // Varsayılan site ayarları oluşturulur
                var setting = new Setting
                {
                    SiteName = "Site Name",
                    Title = "Site Title",
                    ShortDescription = "Short Description of site"
                };

                // Site ayarları veritabanına eklenir
                _context.Settings!.Add(setting);
                // Veritabanındaki değişiklikler kaydedilir
                _context.SaveChanges();
            }
        }
    }
}

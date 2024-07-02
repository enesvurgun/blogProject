using AspNetCoreHero.ToastNotification.Abstractions; // Bildirim servisi için gerekli kütüphane
using projeBlog.Data; // Veri tabanı bağlamı
using projeBlog.Models; // Modeller
using projeBlog.Utilites; // Yardımcı sınıflar
using projeBlog.ViewModels; // Görüntü Modelleri (View Models)
using Microsoft.AspNetCore.Authorization; // Yetkilendirme
using Microsoft.AspNetCore.Identity; // Kimlik yönetimi
using Microsoft.AspNetCore.Mvc; // MVC için gerekli
using Microsoft.EntityFrameworkCore; // Entity Framework Core için gerekli
using X.PagedList; // Sayfalama için gerekli

namespace projeBlog.Areas.Admin.Controllers
{
    [Area("Admin")] // Bu denetleyicinin "Admin" alanına ait olduğunu belirtir
    [Authorize] // Bu denetleyiciye sadece yetkili kullanıcılar erişebilir
    public class PostController : Controller
    {
        private readonly ApplicationDbContext _context; // Veri tabanı bağlamı
        public INotyfService _notification { get; } // Bildirim servisi
        private readonly IWebHostEnvironment _webHostEnvironment; // Web barındırma ortamı
        private readonly UserManager<ApplicationUser> _userManager; // Kullanıcı yönetimi

        // Yapıcı metod (Constructor)
        public PostController(ApplicationDbContext context,
                              INotyfService notyfService,
                              IWebHostEnvironment webHostEnvironment,
                              UserManager<ApplicationUser> userManager)
        {
            _context = context; // Veri tabanı bağlamını ayarlar
            _notification = notyfService; // Bildirim servisini ayarlar
            _webHostEnvironment = webHostEnvironment; // Web barındırma ortamını ayarlar
            _userManager = userManager; // Kullanıcı yönetimini ayarlar
        }

        // Gönderilerin listelenmesi için HTTP GET isteği
        [HttpGet]
        public async Task<IActionResult> Index(int? page)
        {
            var listOfPosts = new List<Post>(); // Gönderi listesini oluşturur

            // Giriş yapan kullanıcıyı bulur
            var loggedInUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);
            var loggedInUserRole = await _userManager.GetRolesAsync(loggedInUser!); // Kullanıcının rolünü alır
            if (loggedInUserRole[0] == WebsiteRoles.WebsiteAdmin)
            {
                // Kullanıcı admin ise tüm gönderileri alır
                listOfPosts = await _context.Posts!.Include(x => x.ApplicationUser).ToListAsync();
            }
            else
            {
                // Kullanıcı admin değilse sadece kendi gönderilerini alır
                listOfPosts = await _context.Posts!.Include(x => x.ApplicationUser).Where(x => x.ApplicationUser!.Id == loggedInUser!.Id).ToListAsync();
            }

            // Gönderileri ViewModel listesine dönüştürür
            var listOfPostsVM = listOfPosts.Select(x => new PostVM()
            {
                Id = x.Id,
                Title = x.Title,
                CreatedDate = x.CreatedDate,
                ThumbnailUrl = x.ThumbnailUrl,
                AuthorName = x.ApplicationUser!.FirstName + " " + x.ApplicationUser.LastName
            }).ToList();

            int pageSize = 5; // Sayfa başına gönderi sayısı
            int pageNumber = (page ?? 1); // Sayfa numarası, eğer belirtilmemişse 1 olur

            // Gönderileri tarihe göre sıralar ve sayfalama uygular
            return View(await listOfPostsVM.OrderByDescending(x => x.CreatedDate).ToPagedListAsync(pageNumber, pageSize));
        }

        // Yeni gönderi oluşturma sayfasını görüntülemek için HTTP GET isteği
        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreatePostVM()); // Boş bir ViewModel ile Create sayfasını döndürür
        }

        // Yeni gönderi oluşturmak için HTTP POST isteği
        [HttpPost]
        public async Task<IActionResult> Create(CreatePostVM vm)
        {
            if (!ModelState.IsValid) { return View(vm); } // Model doğrulama başarısız olursa, aynı View'i geri döndürür

            // Giriş yapan kullanıcıyı bulur
            var loggedInUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);

            var post = new Post
            {
                Title = vm.Title,
                Description = vm.Description,
                ShortDescription = vm.ShortDescription,
                ApplicationUserId = loggedInUser!.Id
            };

            // Slug oluşturma
            if (post.Title != null)
            {
                string slug = vm.Title!.Trim();
                slug = slug.Replace(" ", "-");
                post.Slug = slug + "-" + Guid.NewGuid(); // Benzersiz slug oluşturur
            }

            // Thumbnail yükleme
            if (vm.Thumbnail != null)
            {
                post.ThumbnailUrl = UploadImage(vm.Thumbnail);
            }

            // Yeni gönderiyi veri tabanına ekler ve kaydeder
            await _context.Posts!.AddAsync(post);
            await _context.SaveChangesAsync();
            _notification.Success("Gönderi başarılı bir şekilde oluşturuldu."); // Başarı bildirimi gösterir
            return RedirectToAction("Index"); // Gönderi listesine yönlendirir
        }

        // Gönderi silmek için HTTP POST isteği
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            // Silinecek gönderiyi bulur
            var post = await _context.Posts!.FirstOrDefaultAsync(x => x.Id == id);

            // Giriş yapan kullanıcıyı ve rolünü bulur
            var loggedInUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);
            var loggedInUserRole = await _userManager.GetRolesAsync(loggedInUser!);

            // Kullanıcı admin veya gönderinin sahibi ise gönderiyi siler
            if (loggedInUserRole[0] == WebsiteRoles.WebsiteAdmin || loggedInUser?.Id == post?.ApplicationUserId)
            {
                _context.Posts!.Remove(post!);
                await _context.SaveChangesAsync();
                _notification.Success("Gönderi başarılı bir şekilde silindi."); // Başarı bildirimi gösterir
                return RedirectToAction("Index", "Post", new { area = "Admin" }); // Gönderi listesine yönlendirir
            }
            return View();
        }

        // Gönderi düzenleme sayfasını görüntülemek için HTTP GET isteği
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // Düzenlenecek gönderiyi bulur
            var post = await _context.Posts!.FirstOrDefaultAsync(x => x.Id == id);
            if (post == null)
            {
                _notification.Error("Gönderi bulunamadı."); // Gönderi bulunamazsa hata bildirimi gösterir
                return View();
            }

            // Giriş yapan kullanıcıyı ve rolünü bulur
            var loggedInUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);
            var loggedInUserRole = await _userManager.GetRolesAsync(loggedInUser!);

            // Kullanıcı admin değil ve gönderinin sahibi değilse yetki hatası verir
            if (loggedInUserRole[0] != WebsiteRoles.WebsiteAdmin && loggedInUser!.Id != post.ApplicationUserId)
            {
                _notification.Error("Yetkiniz yok.");
                return RedirectToAction("Index");
            }

            // Gönderi verilerini ViewModel'e aktarır
            var vm = new CreatePostVM()
            {
                Id = post.Id,
                Title = post.Title,
                ShortDescription = post.ShortDescription,
                Description = post.Description,
                ThumbnailUrl = post.ThumbnailUrl,
            };

            // Edit sayfasını ViewModel ile döndürür
            return View(vm);
        }

        // Gönderiyi güncellemek için HTTP POST isteği
        [HttpPost]
        public async Task<IActionResult> Edit(CreatePostVM vm)
        {
            if (!ModelState.IsValid) { return View(vm); } // Model doğrulama başarısız olursa, aynı View'i geri döndürür
            var post = await _context.Posts!.FirstOrDefaultAsync(x => x.Id == vm.Id);
            if (post == null)
            {
                _notification.Error("Gönderi bulunamadı."); // Gönderi bulunamazsa hata bildirimi gösterir
                return View();
            }

            // Gönderi verilerini günceller
            post.Title = vm.Title;
            post.ShortDescription = vm.ShortDescription;
            post.Description = vm.Description;

            // Yeni bir thumbnail yüklenmişse, thumbnail URL'sini günceller
            if (vm.Thumbnail != null)
            {
                post.ThumbnailUrl = UploadImage(vm.Thumbnail);
            }

            // Değişiklikleri veri tabanına kaydeder
            await _context.SaveChangesAsync();
            _notification.Success("Gönderi başarılı bir şekilde güncellendi."); // Başarı bildirimi gösterir
            return RedirectToAction("Index", "Post", new { area = "Admin" }); // Gönderi listesine yönlendirir
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

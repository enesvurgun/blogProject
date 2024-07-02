using AspNetCoreHero.ToastNotification.Abstractions; // Bildirim servisi için gerekli kütüphane
using projeBlog.Models; // Modeller
using projeBlog.Utilites; // Yardımcı sınıflar
using projeBlog.ViewModels; // Görüntü Modelleri (View Models)
using Microsoft.AspNetCore.Authorization; // Yetkilendirme
using Microsoft.AspNetCore.Identity; // Kimlik yönetimi
using Microsoft.AspNetCore.Mvc; // MVC için gerekli
using Microsoft.EntityFrameworkCore; // Entity Framework Core için gerekli

namespace projeBlog.Areas.Admin.Controllers
{
    [Area("Admin")] // Bu denetleyicinin "Admin" alanına ait olduğunu belirtir
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager; // Kullanıcı yönetimi
        private readonly SignInManager<ApplicationUser> _signInManager; // Oturum yönetimi
        public INotyfService _notification { get; } // Bildirim servisi

        // Yapıcı metod (Constructor)
        public UserController(UserManager<ApplicationUser> userManager,
                              SignInManager<ApplicationUser> signInManager,
                              INotyfService notyfService)
        {
            _userManager = userManager; // Kullanıcı yönetimini ayarlar
            _signInManager = signInManager; // Oturum yönetimini ayarlar
            _notification = notyfService; // Bildirim servisini ayarlar
        }

        [Authorize(Roles = "Admin")] // Sadece Admin rolüne sahip kullanıcılar erişebilir
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync(); // Tüm kullanıcıları alır

            // Kullanıcıları ViewModel listesine dönüştürür
            var vm = users.Select(x => new UserVM()
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                UserName = x.UserName,
                Email = x.Email,
            }).ToList();

            // Her kullanıcı için rol bilgilerini alır
            foreach (var user in vm)
            {
                var singleUser = await _userManager.FindByIdAsync(user.Id);
                var role = await _userManager.GetRolesAsync(singleUser);
                user.Role = role.FirstOrDefault();
            }

            return View(vm); // View'e ViewModel'i gönderir
        }

        [Authorize(Roles = "Admin")] // Sadece Admin rolüne sahip kullanıcılar erişebilir
        [HttpGet]
        public async Task<IActionResult> ResetPassword(string id)
        {
            var existingUser = await _userManager.FindByIdAsync(id); // Kullanıcıyı ID ile bulur
            if (existingUser == null)
            {
                _notification.Error("Böyle bir kullanıcı yok."); // Kullanıcı bulunamazsa hata bildirimi gösterir
                return View();
            }
            var vm = new ResetPasswordVM()
            {
                Id = existingUser.Id,
                UserName = existingUser.UserName
            };
            return View(vm); // Kullanıcı bilgilerini ViewModel'e aktarır ve View'e gönderir
        }

        [Authorize(Roles = "Admin")] // Sadece Admin rolüne sahip kullanıcılar erişebilir
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM vm)
        {
            if (!ModelState.IsValid) { return View(vm); } // Model doğrulama başarısız olursa, aynı View'i geri döndürür
            var existingUser = await _userManager.FindByIdAsync(vm.Id); // Kullanıcıyı ID ile bulur
            if (existingUser == null)
            {
                _notification.Error("Böyle bir kullanıcı yok."); // Kullanıcı bulunamazsa hata bildirimi gösterir
                return View(vm);
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(existingUser); // Şifre sıfırlama token'ı oluşturur
            var result = await _userManager.ResetPasswordAsync(existingUser, token, vm.NewPassword); // Şifreyi sıfırlar
            if (result.Succeeded)
            {
                _notification.Success("Şifre başarılı bir şekilde güncellendi."); // Başarı bildirimi gösterir
                return RedirectToAction(nameof(Index)); // Kullanıcı listesine yönlendirir
            }
            return View(vm); // Hata durumunda aynı View'i geri döndürür
        }

        [HttpGet("Register")] // Kayıt sayfasını görüntülemek için HTTP GET isteği
        public IActionResult Register()
        {
            return View(new RegisterVM()); // Boş bir ViewModel ile Register sayfasını döndürür
        }

        [HttpPost("Register")] // Kayıt işlemi için HTTP POST isteği
        public async Task<IActionResult> Register(RegisterVM vm)
        {
            if (!ModelState.IsValid) { return View(vm); } // Model doğrulama başarısız olursa, aynı View'i geri döndürür
            var checkUserByEmail = await _userManager.FindByEmailAsync(vm.Email); // Email ile kullanıcıyı kontrol eder
            if (checkUserByEmail != null)
            {
                _notification.Error("Email zaten mevcut."); // Email zaten mevcutsa hata bildirimi gösterir
                return View(vm);
            }
            var checkUserByUsername = await _userManager.FindByNameAsync(vm.UserName); // Kullanıcı adı ile kullanıcıyı kontrol eder
            if (checkUserByUsername != null)
            {
                _notification.Error("Kullanıcı adı zaten mevcut."); // Kullanıcı adı zaten mevcutsa hata bildirimi gösterir
                return View(vm);
            }

            // Yeni kullanıcı oluşturur
            var applicationUser = new ApplicationUser()
            {
                Email = vm.Email,
                UserName = vm.UserName,
                FirstName = vm.FirstName,
                LastName = vm.LastName
            };

            var result = await _userManager.CreateAsync(applicationUser, vm.Password); // Kullanıcıyı oluşturur
            if (result.Succeeded)
            {
                if (vm.IsAdmin)
                {
                    await _userManager.AddToRoleAsync(applicationUser, WebsiteRoles.WebsiteAdmin); // Kullanıcıyı admin rolüne ekler
                }
                else
                {
                    await _userManager.AddToRoleAsync(applicationUser, WebsiteRoles.WebsiteAuthor); // Kullanıcıyı yazar rolüne ekler
                }
                _notification.Success("Kayıt işlemi başarılı bir şekilde gerçekleşti."); // Başarı bildirimi gösterir
                return RedirectToAction("Index", "User", new { area = "Admin" }); // Kullanıcı listesine yönlendirir
            }
            return View(vm); // Hata durumunda aynı View'i geri döndürür
        }

        [HttpGet("Login")] // Giriş sayfasını görüntülemek için HTTP GET isteği
        public IActionResult Login()
        {
            if (!HttpContext.User.Identity!.IsAuthenticated)
            {
                return View(new LoginVM()); // Boş bir ViewModel ile Login sayfasını döndürür
            }
            return RedirectToAction("Index", "Post", new { area = "Admin" }); // Kullanıcı zaten giriş yapmışsa, gönderi listesine yönlendirir
        }

        [HttpPost("Login")] // Giriş işlemi için HTTP POST isteği
        public async Task<IActionResult> Login(LoginVM vm)
        {
            if (!ModelState.IsValid) { return View(vm); } // Model doğrulama başarısız olursa, aynı View'i geri döndürür
            var existingUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == vm.Username); // Kullanıcıyı kullanıcı adı ile bulur
            if (existingUser == null)
            {
                _notification.Error("Böyle bir kullanıcı yok."); // Kullanıcı bulunamazsa hata bildirimi gösterir
                return View(vm);
            }
            var verifyPassword = await _userManager.CheckPasswordAsync(existingUser, vm.Password); // Şifreyi kontrol eder
            if (!verifyPassword)
            {
                _notification.Error("Şifre eşleşmiyor."); // Şifre eşleşmezse hata bildirimi gösterir
                return View(vm);
            }
            await _signInManager.PasswordSignInAsync(vm.Username, vm.Password, vm.RememberMe, true); // Kullanıcıyı oturum açtırır
            _notification.Success("Giriş Başarılı"); // Başarı bildirimi gösterir
            return RedirectToAction("Index", "Post", new { area = "Admin" }); // Gönderi listesine yönlendirir
        }

        [HttpPost]
        [Authorize] // Yetkili kullanıcılar erişebilir
        public IActionResult Logout()
        {
            _signInManager.SignOutAsync(); // Kullanıcıyı oturumdan çıkarır
            _notification.Success("Başarılı bir şekilde çıkış yapıldı."); // Başarı bildirimi gösterir
            return RedirectToAction("Index", "Home", new { area = "" }); // Ana sayfaya yönlendirir
        }

        [HttpGet("AccessDenied")] // Yetkisiz erişim sayfasını görüntülemek için HTTP GET isteği
        [Authorize] // Yetkili kullanıcılar erişebilir
        public IActionResult AccessDenied()
        {
            return View(); // Access Denied sayfasını döndürür
        }
    }
}

using Microsoft.Build.Framework;
//uygulama katmanları arasında veri taşımak ve veri aktarımını daha etkili bir şekilde yönetmek için
//viewmodels kullandım.
namespace projeBlog.ViewModels
{
    public class LoginVM
    {
        [Required]
        public string? Username { get; set; }
        [Required]
        public string? Password { get; set; }
        public bool RememberMe { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
//uygulama katmanları arasında veri taşımak ve veri aktarımını daha etkili bir şekilde yönetmek için
//viewmodels kullandım.
namespace projeBlog.ViewModels
{
    public class RegisterVM
    {
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        public string? UserName { get; set; }
        [Required]
        public string? Password { get; set; }
        public bool IsAdmin { get; set; }
    }
}

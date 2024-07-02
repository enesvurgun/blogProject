using System.ComponentModel.DataAnnotations;
//uygulama katmanları arasında veri taşımak ve veri aktarımını daha etkili bir şekilde yönetmek için
//viewmodels kullandım.
namespace projeBlog.ViewModels
{
    public class ResetPasswordVM
    {
        public string? Id { get; set; }
        public string? UserName { get; set; }
        [Required]
        public string? NewPassword { get; set; }
        [Compare(nameof(NewPassword))]
        [Required]
        public string? ConfirmPasswor { get; set; }
    }
}

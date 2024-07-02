using Microsoft.Build.Framework;
//uygulama katmanları arasında veri taşımak ve veri aktarımını daha etkili bir şekilde yönetmek için
//viewmodels kullandım.
namespace projeBlog.ViewModels
{
    public class PageVM
    {
        public int Id { get; set; }
        [Required]
        public string? Title { get; set; }
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }
        public string? ThumbnailUrl { get; set; }
        public IFormFile? Thumbnail { get; set; }
    }
}

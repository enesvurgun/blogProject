namespace projeBlog.ViewModels
{//uygulama katmanları arasında veri taşımak ve veri aktarımını daha etkili bir şekilde yönetmek için
    //viewmodels kullandım.
    public class SettingVM
    {
        public int Id { get; set; }
        public string? SiteName { get; set; }
        public string? Title { get; set; }
        public string? ShortDescription { get; set; }
        public string? ThumbnailUrl { get; set; }
     
        public IFormFile? Thumbnail { get; set; }
    }
}

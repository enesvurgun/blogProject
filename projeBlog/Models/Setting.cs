namespace projeBlog.Models
{//models klasöründeki her sınıf aslında view sayfalarına gönderilen birer parametre olarak düşünebiliriz.
    public class Setting
    {
        public int Id { get; set; }
        public string? SiteName { get; set; }
        public string? Title { get; set; }
        public string? ShortDescription { get; set; }
        public string? ThumbnailUrl { get; set; }
      
    }
}

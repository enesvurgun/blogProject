namespace projeBlog.ViewModels
{//uygulama katmanları arasında veri taşımak ve veri aktarımını daha etkili bir şekilde yönetmek için
    //viewmodels kullandım.
    public class ContactVM
    {
        public ContactVM()
        {
            PageVM = new PageVM();
        }
        public PageVM PageVM { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Subject { get; set; }
        public string? Message { get; set; }
    }
}

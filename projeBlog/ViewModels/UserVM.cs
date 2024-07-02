namespace projeBlog.ViewModels
{//uygulama katmanları arasında veri taşımak ve veri aktarımını daha etkili bir şekilde yönetmek için
    //viewmodels kullandım.
    public class UserVM
    {
        public string? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
    }
}

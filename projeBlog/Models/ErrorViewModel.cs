namespace projeBlog.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}//models klasöründeki her sýnýf aslýnda view sayfalarýna gönderilen birer parametre olarak düþünebiliriz.
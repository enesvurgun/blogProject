namespace projeBlog.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}//models klas�r�ndeki her s�n�f asl�nda view sayfalar�na g�nderilen birer parametre olarak d���nebiliriz.
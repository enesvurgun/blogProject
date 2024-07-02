using System.ComponentModel.DataAnnotations;
//models klasöründeki her sınıf aslında view sayfalarına gönderilen birer parametre olarak düşünebiliriz.
namespace projeBlog.Models
{
    public class Contact
    {
         
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Subject { get; set; }
        public string? Message { get; set; }
    }
}

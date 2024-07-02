using Microsoft.AspNetCore.Identity;


//models klasöründeki her sınıf aslında view sayfalarına gönderilen birer parametre olarak düşünebiliriz.
namespace projeBlog.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        //relation
        public List<Post>? Posts { get; set; }
    }
}

using projeBlog.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

//bu sınıf veritabanıyla doğrudan iletişim kuran sınıftır. verilerle iletişime geçerken iş katmanında
//bu sınıf oluşturulup dependency injection ile daha sonra database işlemleri bu sınıf nesnesi üzerinden
//yapılır.
namespace projeBlog.Data
{
    public class ApplicationDbContext:IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {
        }
        public DbSet<ApplicationUser>? ApplicationUsers { get; set; }
        public DbSet<Post>? Posts { get; set; }
        public DbSet<Page>? Pages { get; set; }
        public DbSet<Setting>? Settings { get; set; }
        public DbSet<Contact>? Contacts { get; set; }

    }
}

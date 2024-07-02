using MimeKit; // MimeKit kütüphanesini kullanır. Bu kütüphane e-posta mesajlarını oluşturmak için kullanılır.
using MailKit.Net.Smtp; // MailKit kütüphanesini kullanır. Bu kütüphane SMTP sunucusu ile bağlantı kurmak için kullanılır.

namespace projeBlog.Utilites // projeBlog.Utilites ad alanı
{
    public class MailSender // MailSender sınıfı
    {
        public static async Task Send(string from, string subject, string message) // Send adında bir static metot. E-posta göndermek için kullanılır.
        {
            var mail = new MimeMessage(); // Yeni bir MimeMessage nesnesi oluşturur.
            mail.From.Add(new MailboxAddress("Gönderen", from)); // Gönderen adresini ve adını ekler.
            mail.To.Add(new MailboxAddress("Recipient Name", "enesvrgun@gmail.com")); // Alıcı adresini ve adını ekler.
            mail.Subject = subject; // E-posta konusunu ayarlar.
            mail.Body = new TextPart("plain") // E-posta gövdesini ayarlar.
            {
                Text = message // E-posta metnini ayarlar.
            };

            using (var client = new SmtpClient()) // Yeni bir SmtpClient nesnesi oluşturur ve using bloğu içinde kullanır.
            {
                try
                {
                    // Gmail SMTP server ayarları
                    var smtpServer = "smtp.gmail.com"; // SMTP sunucusunun adresini belirler.
                    var smtpPort = 587; // SMTP sunucusunun portunu belirler.
                    var smtpUser = "enesvrgun@gmail.com"; // SMTP sunucusu için kullanıcı adını belirler.
                    var smtpPass = "terv myou mplg smzi"; // SMTP sunucusu için şifreyi belirler.

                    // SMTP sunucusuna bağlanır.
                    await client.ConnectAsync(smtpServer, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                    // SMTP sunucusunda kimlik doğrulaması yapar.
                    await client.AuthenticateAsync(smtpUser, smtpPass);
                    // E-postayı gönderir.
                    await client.SendAsync(mail);
                    // SMTP sunucusundan bağlantıyı keser.
                    await client.DisconnectAsync(true);
                    Console.WriteLine("Email sent successfully!"); // Başarılı gönderim mesajı.
                }
                catch (Exception ex) // Hata durumunda çalışacak blok.
                {
                    Console.WriteLine($"An error occurred: {ex.Message}"); // Hata mesajını konsola yazdırır.
                }
            }
        }
    }
}

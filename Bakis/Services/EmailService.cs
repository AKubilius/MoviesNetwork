using System.Net.Mail;
using System.Net;

namespace Bakis.Services
{
    public class EmailService : IEmailService
    {
        public async Task SendEmailAsync(string to, string subject, string message)
        {
            
            using var client = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                Credentials = new NetworkCredential("cinemaconnectapi@gmail.com", "mscxfkvtbhdwvgxd"),
                EnableSsl = true
            };
            var emailMessage = new MailMessage("cinemaconnectapi@gmail.com", to, subject, message);
            emailMessage.IsBodyHtml = true;
            await client.SendMailAsync(emailMessage);
        }
    }
}

using System.Net.Mail;

namespace TheBlog.MVC.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(MailMessage message, string emailToSendTo);

        Task<(bool success, string message)> SendResetPasswordEmailAsync(string token, string email);
    }
}
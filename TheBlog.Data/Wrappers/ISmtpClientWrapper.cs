using System.Net.Mail;

namespace TheBlog.MVC.Wrappers
{
    public interface ISmtpClientWrapper
    {
        Task SendMailAsync(MailMessage message);
    }
}
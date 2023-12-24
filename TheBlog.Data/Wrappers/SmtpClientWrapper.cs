using System.Diagnostics.CodeAnalysis;
using System.Net.Mail;

namespace TheBlog.MVC.Wrappers
{
    [ExcludeFromCodeCoverage]
    public class SmtpClientWrapper : ISmtpClientWrapper
    {
        private readonly SmtpClient _smtpClient;

        public SmtpClientWrapper(SmtpClient smtpClient)
        {
            _smtpClient = smtpClient;
        }

        public async Task SendMailAsync(MailMessage message)
        {
            if (message == null)
                return;

            await _smtpClient.SendMailAsync(message);
        }
    }
}
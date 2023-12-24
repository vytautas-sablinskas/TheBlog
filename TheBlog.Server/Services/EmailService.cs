using System.Net.Mail;
using TheBlog.Data.Utilities;
using TheBlog.MVC.Wrappers;

namespace TheBlog.MVC.Services
{
    public class EmailService : IEmailService
    {
        private readonly ISmtpClientWrapper _smtpClientWrapper;
        private readonly ILinkGenerationService _linkGenerationService;

        public EmailService(ISmtpClientWrapper smptClientWrapper, ILinkGenerationService linkGenerationService)
        {
            _smtpClientWrapper = smptClientWrapper;
            _linkGenerationService = linkGenerationService;
        }

        public async Task SendEmailAsync(MailMessage message, string emailToSendTo)
        {
            if (message == null)
                return;

            message.To.Add(emailToSendTo);

            await _smtpClientWrapper.SendMailAsync(message);
        }

        public async Task<(bool success, string message)> SendResetPasswordEmailAsync(string token, string email)
        {
            var resetLink = await _linkGenerationService.GeneratePasswordResetLink(token, email);

            try
            {
                await SendEmailAsync(EmailMessageTemplates.CreatePasswordResetMessage(resetLink), email);
            }
            catch
            {
                return (success: false, message: "An error occurred while sending email.");
            }

            return (success: true, message: "Password reset link has been sent to your email.");
        }
    }
}
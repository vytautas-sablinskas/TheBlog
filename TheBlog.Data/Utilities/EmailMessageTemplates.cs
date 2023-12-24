using System.Diagnostics.CodeAnalysis;
using System.Net.Mail;

namespace TheBlog.Data.Utilities
{
    [ExcludeFromCodeCoverage]
    public class EmailMessageTemplates
    {
        public static MailMessage CreatePasswordResetMessage(string resetLink)
        {
            return new MailMessage
            {
                From = new MailAddress("theblog.email.sender@gmail.com"),
                Subject = "[The Blog]: Reset password link",
                Body = $"<h1>Click <a href='{resetLink}'>here</a> to reset your password.</h1>",
                IsBodyHtml = true
            };
        }
    }
}
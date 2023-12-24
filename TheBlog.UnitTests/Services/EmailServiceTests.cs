using Moq;
using System.Net.Mail;
using TheBlog.Data.Utilities;
using TheBlog.MVC.Services;
using TheBlog.MVC.Wrappers;

namespace TheBlog.UnitTests.Services
{
    [TestFixture]
    public class EmailServiceTests
    {
        private Mock<ISmtpClientWrapper> _mockSmtpClientWrapper;
        private Mock<ILinkGenerationService> _mockLinkGenerationService;
        private EmailService _sut;

        private const string _token = "fake token";
        private const string _email = "fakeEmail@gmail.com";
        private const string _link = "link/";

        [SetUp]
        public void SetUp()
        {
            _mockSmtpClientWrapper = new Mock<ISmtpClientWrapper>();
            _mockLinkGenerationService = new Mock<ILinkGenerationService>();
            _sut = new EmailService(_mockSmtpClientWrapper.Object, _mockLinkGenerationService.Object);
        }

        [Test]
        public async Task SendEmailAsync_NullMessage_DoesNotSendEmail()
        {
            MailMessage message = null;

            await _sut.SendEmailAsync(message, _email);

            _mockSmtpClientWrapper.Verify(m => m.SendMailAsync(message), Times.Never);
        }

        [Test]
        public async Task SendEmailAsync_ValidInput_SendsEmailToCorrectPerson()
        {
            var message = EmailMessageTemplates.CreatePasswordResetMessage(_link);

            await _sut.SendEmailAsync(message, _email);

            _mockSmtpClientWrapper.Verify(m => m.SendMailAsync(message), Times.Once);
            Assert.That(message.To, Has.Count.EqualTo(1));
        }

        [Test]
        public async Task SendResetPasswordEmailAsync_ValidInput_ShouldSendCorrectMessageToCorrectEmail()
        {
            _mockLinkGenerationService.Setup(m => m.GeneratePasswordResetLink(_token, _email))
                                      .ReturnsAsync(_link);

            (bool resetEmailWasSent, string message) = await _sut.SendResetPasswordEmailAsync(_token, _email);

            _mockSmtpClientWrapper.Verify(m => m.SendMailAsync(It.IsAny<MailMessage>()), Times.Once());
            Assert.Multiple(() =>
            {
                Assert.That(resetEmailWasSent);
                Assert.That(message, Is.EqualTo("Password reset link has been sent to your email."));
            });
        }

        [Test]
        public async Task SendResetPasswordEmailAsync_ErrorThrown_ShouldReturnCorrectStatusAndMessage()
        {
            _mockLinkGenerationService.Setup(m => m.GeneratePasswordResetLink(_token, _email))
                                      .ReturnsAsync(_link);
            _mockSmtpClientWrapper.Setup(m => m.SendMailAsync(It.IsAny<MailMessage>()))
                                  .ThrowsAsync(It.IsAny<Exception>());

            (bool resetEmailWasSent, string message) = await _sut.SendResetPasswordEmailAsync(_token, _email);

            _mockSmtpClientWrapper.Verify(m => m.SendMailAsync(It.IsAny<MailMessage>()), Times.Once());
            Assert.Multiple(() =>
            {
                Assert.That(!resetEmailWasSent);
                Assert.That(message, Is.EqualTo("An error occurred while sending email."));
            });
        }
    }
}
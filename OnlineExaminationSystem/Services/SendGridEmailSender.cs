using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OnlineExaminationSystem.Services;
using SendGrid;
using SendGrid.Helpers.Errors.Model;
using SendGrid.Helpers.Mail;

public class SendGridEmailSender : IEmailSender
{
    private readonly SendGridEmailSenderOptions _senderOptions;
    private readonly ILogger<SendGridEmailSender> _logger;

    public SendGridEmailSender(IOptions<SendGridEmailSenderOptions> senderOptions, ILogger<SendGridEmailSender> logger)
    {
        _senderOptions = senderOptions.Value;
        _logger = logger;
    }

    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        try
        {
            var apiKey = _senderOptions.ApiKey;
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(_senderOptions.SenderEmail, _senderOptions.SenderName);
            var to = new EmailAddress(email, "Recipient");
            var msg = MailHelper.CreateSingleEmail(from, to, subject, htmlMessage, htmlMessage);

            return client.SendEmailAsync(msg);
        }
        catch (SendGridInternalException ex)
        {
            _logger.LogError(ex, "SendGrid internal exception occurred while sending email.");
            throw;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error occurred while sending email.");
            throw;
        }

    }
}

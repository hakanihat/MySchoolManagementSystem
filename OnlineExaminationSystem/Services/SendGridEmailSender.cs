using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using OnlineExaminationSystem.Services;
using SendGrid;
using SendGrid.Helpers.Mail;

public class SendGridEmailSender : IEmailSender
{
    private readonly SendGridEmailSenderOptions _senderOptions;

    public SendGridEmailSender(IOptions<SendGridEmailSenderOptions> senderOptions)
    {
        _senderOptions = senderOptions.Value;
    }

    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var apiKey = _senderOptions.ApiKey;
        var client = new SendGridClient(apiKey);
        var from = new EmailAddress(_senderOptions.SenderEmail, _senderOptions.SenderName);
        var to = new EmailAddress(email, "Recipient");

        var msg = MailHelper.CreateSingleEmail(from, to, subject, htmlMessage, htmlMessage);

        return client.SendEmailAsync(msg);
    }
}

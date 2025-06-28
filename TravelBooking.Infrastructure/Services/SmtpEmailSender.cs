using System.Net;
using System.Net.Mail;
using AttributeBasedRegistration;
using AttributeBasedRegistration.Attributes;
using Microsoft.Extensions.Options;
using TravelBooking.Application.Common.Interfaces;
using TravelBooking.Application.Common.Models;
using TravelBooking.Infrastructure.Options;

namespace TravelBooking.Infrastructure.Services;

[ServiceImplementation]
[RegisterAs<IEmailSender>]
[Lifetime(ServiceLifetime.InstancePerLifetimeScope)]
public class SmtpEmailSender:IEmailSender
{
    private readonly SmtpOptions _smtpOptions;

    public SmtpEmailSender(IOptions<SmtpOptions> smtpOptions)
    {
        _smtpOptions = smtpOptions.Value;
    }
    
    public async Task SendEmailAsync(string email, string subject, string message, PdfResult? pdfResult=null, CancellationToken cancellationToken = default)
    {
        var smtp = new SmtpClient(_smtpOptions.Host, _smtpOptions.Port);
        smtp.Credentials = new NetworkCredential(_smtpOptions.UserName, _smtpOptions.Password);
        smtp.EnableSsl = true;

        var mail = new MailMessage(_smtpOptions.FromEmail, email, subject, message);
        mail.IsBodyHtml = true;
        
        if (pdfResult != null)
        {
            var pdfStream = new MemoryStream(pdfResult.Data);
            pdfStream.Position = 0;
            mail.Attachments.Add(new Attachment(pdfStream, pdfResult.FileName, "application/pdf"));
        }
        
        await smtp.SendMailAsync(mail,cancellationToken);
    }
}
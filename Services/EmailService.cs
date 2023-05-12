using System.Net.Mail;
using CrucibleBlog.Models;
using MailKit.Security;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;

namespace CrucibleBlog.Services;

public class EmailService : IEmailSender
{
    private readonly EmailSettings _emailSettings;
    public EmailService(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        try
        {
            var emailAddress = _emailSettings.EmailAddress ?? Environment.GetEnvironmentVariable("EmailAddress");
            var emailPassword = _emailSettings.EmailPassword ?? Environment.GetEnvironmentVariable("EmailPassword");
            var emailHost = _emailSettings.EmailHost ?? Environment.GetEnvironmentVariable("EmailHost");
            var emailPort = _emailSettings.EmailPort != 0 ? _emailSettings.EmailPort : int.Parse(Environment.GetEnvironmentVariable("EmailPort")!);

            MimeMessage newEmail = new MimeMessage();
            newEmail.Sender = MailboxAddress.Parse(emailAddress);

            foreach (string address in email.Split(";"))
            {
                newEmail.To.Add(MailboxAddress.Parse(address));
            }

            newEmail.Subject = subject;

            BodyBuilder emailBody = new BodyBuilder();
            emailBody.HtmlBody = htmlMessage;

            newEmail.Body = emailBody.ToMessageBody();

            using MailKit.Net.Smtp.SmtpClient smtpClient = new MailKit.Net.Smtp.SmtpClient();

            try
            {
                var host = _emailSettings.EmailHost ?? Environment.GetEnvironmentVariable("EmailHost");
                var port = _emailSettings.EmailPort != 0 ? _emailSettings.EmailPort : int.Parse(Environment.GetEnvironmentVariable("EmailPort")!);
                var password = _emailSettings.EmailPassword ?? Environment.GetEnvironmentVariable("EmailPassword");

                await smtpClient.ConnectAsync(emailHost, emailPort, SecureSocketOptions.StartTls);
                await smtpClient.AuthenticateAsync(emailAddress, emailPassword);

                await smtpClient.SendAsync(newEmail);
                await smtpClient.DisconnectAsync(true);

            }
            catch (Exception ex)
            {
                var error = ex.Message;
                throw;
            }
        }
        catch (Exception)
        {

            throw;
        }
    }
}

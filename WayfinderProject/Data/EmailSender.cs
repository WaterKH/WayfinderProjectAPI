using Microsoft.AspNetCore.Identity.UI.Services;
using SendGrid;
using SendGrid.Helpers.Mail;
using WayfinderProject.Data.Models;

namespace WayfinderProject.Data
{

    public class EmailSender : IEmailSender
    {
        public EmailSender()
        {
            Options = new AuthMessageSenderOptions
            {
                SendGridKey = Environment.GetEnvironmentVariable("EmailApiKey")
            };
        }

        public AuthMessageSenderOptions Options { get; } //Set with Env.

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            if (string.IsNullOrEmpty(Options.SendGridKey))
            {
                throw new Exception("Null SendGridKey");
            }
            await Execute(Options.SendGridKey, subject, message, toEmail);
        }

        public async Task Execute(string apiKey, string subject, string message, string toEmail)
        {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("thewayfinderprojectkh@gmail.com", "The Wayfinder Project - Password Recovery"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(toEmail));

            // Disable click tracking.
            // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
            msg.SetClickTracking(false, false);
            var response = await client.SendEmailAsync(msg);
        }
    }
}
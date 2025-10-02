using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Contactly.Services
{
    public class EmailService
    {
        private readonly string apiKey;

        public EmailService()
        {
            // Load API key from environment variable
            apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");

            if (string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException("SendGrid API key not found in environment variables.");
            }
        }

        public async Task SendWelcomeEmail(string toEmail, string userName)
        {
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("big44nhce@gmail.com", "Contactly Team");
            var to = new EmailAddress(toEmail, userName);
            var subject = "Welcome to Contactly!";

            // Load HTML template
            string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "welcome.html");

            if (!File.Exists(templatePath))
            {
                throw new FileNotFoundException("Email template not found at: " + templatePath);
            }

            string htmlContent = File.ReadAllText(templatePath);
            htmlContent = htmlContent.Replace("{{name}}", userName);

            // Fallback plain text version
            var plainTextContent = $"Hi {userName}, thanks for registering with Contactly!";

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);

            if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
            {
                throw new Exception($"Failed to send email. Status: {response.StatusCode}");
            }
        }
    }
}


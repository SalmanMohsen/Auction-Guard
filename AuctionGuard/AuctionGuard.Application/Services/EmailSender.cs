using AuctionGuard.Application.IServices;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionGuard.Application.Services
{
    /// <summary>
    /// A development implementation of IEmailSender that writes emails to the console/debug output.
    /// </summary>
    public class EmailSender : IEmailSender
    {
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(ILogger<EmailSender> logger)
        {
            _logger = logger;
        }

        public Task SendEmailAsync(string toEmail, string subject, string message)
        {
            // For development, we just log the email to the console.
            // In production, you would replace this with a real email client
            // like SendGrid, MailKit, etc.
            _logger.LogWarning($"--- BEGIN EMAIL ---");
            _logger.LogInformation($"To: {toEmail}");
            _logger.LogInformation($"Subject: {subject}");
            _logger.LogInformation($"Body: {message}");
            _logger.LogWarning($"--- END EMAIL ---");

            return Task.CompletedTask;
        }
    }
}

using AuctionGuard.Application.IServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AuctionGuard.Infrastructure.Services
{
    /// <summary>
    /// A production-ready implementation of IEmailSender using the Mailgun service.
    /// </summary>
    public class MailgunEmailSender : IEmailSender
    {
        private readonly ILogger<MailgunEmailSender> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public MailgunEmailSender(ILogger<MailgunEmailSender> logger, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            var apiKey = _configuration["Mailgun:ApiKey"];
            var domain = _configuration["Mailgun:Domain"];
            var fromEmail = _configuration["EmailSettings:SenderEmail"];
            var fromName = _configuration["EmailSettings:SenderName"];

            if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(domain))
            {
                _logger.LogError("Mailgun API Key or Domain is not configured. Email not sent.");
                return;
            }

            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Post, $"https://api.mailgun.net/v3/{domain}/messages");

            var basicAuth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"api:{apiKey}"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", basicAuth);

            var content = new MultipartFormDataContent
            {
                { new StringContent(fromEmail), "from" },
                { new StringContent(toEmail), "to" },
                { new StringContent(subject), "subject" },
                { new StringContent(htmlMessage), "html" }
            };
            request.Content = content;

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Email to {ToEmail} queued for delivery successfully via Mailgun!", toEmail);
            }
            else
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to send email via Mailgun. Status Code: {StatusCode}, Response: {ResponseBody}",
                    response.StatusCode,
                    responseBody);
            }
        }
    }
}

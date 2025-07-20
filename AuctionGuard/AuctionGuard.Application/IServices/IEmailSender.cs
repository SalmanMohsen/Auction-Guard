using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionGuard.Application.IServices
{
    /// <summary>
    /// Defines the contract for a service that sends emails.
    /// </summary>
    public interface IEmailSender
    {
        /// <summary>
        /// Sends an email.
        /// </summary>
        /// <param name="toEmail">The recipient's email address.</param>
        /// <param name="subject">The subject of the email.</param>
        /// <param name="message">The HTML or plain text content of the email.</param>
        Task SendEmailAsync(string toEmail, string subject, string message);
    }
}

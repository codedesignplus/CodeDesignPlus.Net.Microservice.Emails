// using System;
// using System.Net;
// using System.Net.Mail;

// using MailKit.Net.Smtp;
// using MimeKit;


// namespace CodeDesignPlus.Net.Microservice.Smtp.Infrastructure.Services.Smtp;

// public class Smtp : ISmtp
// {
//     public Smtp()
//     {
//     }

//     public async Task SendEmail(Guid id, List<string> to, List<string> cc, List<string> bcc, string subject, string body, string from, string alias, List<string> attachments, bool isHtml, Dictionary<string, string> values, string status, string server, CancellationToken cancellationToken)
//     {
//         // Create a new MailMessage object
//         var message = new MailMessage();
//         message.To.Add(string.Join(",", to));
//         message.CC.Add(string.Join(",", cc));
//         message.Bcc.Add(string.Join(",", bcc));
//         message.From = new MailAddress(from, alias);
//         message.Subject = subject;
//         message.Body = body;
//         message.IsBodyHtml = isHtml;

//         // Create an SmtpClient object
//         var client = new SmtpClient
//         {
//             Credentials = new NetworkCredential("your_email_address", "your_password"),
//             Port = 587,
//             Host = "smtp.office365.com",
//             EnableSsl = true
//         };

//         try
//         {
//             await client.SendMailAsync(message, cancellationToken: cancellationToken);
//             Console.WriteLine("Email sent successfully!");
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine($"Error sending email: {ex.Message}");
//         }
//     }
// }
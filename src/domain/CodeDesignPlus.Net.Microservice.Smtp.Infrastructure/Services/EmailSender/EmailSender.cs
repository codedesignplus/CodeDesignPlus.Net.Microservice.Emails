using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Infrastructure.Services.GraphClient;
using CodeDesignPlus.Net.Microservice.Smtp.Domain.Models;
using CodeDesignPlus.Net.Microservice.Smtp.Domain.Services;
using CodeDesignPlus.Net.Microservice.Smtp.Infrastructure.Options;
using Microsoft.Graph.Models;
using Microsoft.Graph.Users.Item.SendMail;

namespace CodeDesignPlus.Net.Microservice.Smtp.Infrastructure.Services.EmailSender;

public class EmailSender(IGraphClient graphClient, IOptions<EmailOptions> options) : IEmailSender
{
    public async Task<EmailResponse> SendEmail(EmailMessage emailMessage, CancellationToken cancellationToken)
    {
        var message = new Message()
        {
            Subject = emailMessage.Subject,
            Body = new ItemBody
            {
                ContentType = emailMessage.IsHtm ? BodyType.Html : BodyType.Text,
                Content = emailMessage.Body
            },
            ToRecipients = BuildRecipients(emailMessage.To),
            CcRecipients = BuildRecipients(emailMessage.Cc),
            BccRecipients = BuildRecipients(emailMessage.Bcc),
            Attachments = BuildAttachments(emailMessage.Attachments)
        };

        if (!string.IsNullOrEmpty(emailMessage.From))
        {
            message.From = new Recipient
            {
                EmailAddress = new EmailAddress
                {
                    Address = emailMessage.From,
                    Name = emailMessage.Alias
                }
            };
        }

        var request = new SendMailPostRequestBody
        {
            Message = message,
            SaveToSentItems = true
        };

        try
        {
            await graphClient.Client.Users[options.Value.UserIdWithLicense].SendMail.PostAsync(request, cancellationToken: cancellationToken);

        }
        catch (Microsoft.Graph.Models.ODataErrors.ODataError ex)
        {
            return EmailResponse.Create(ex.Error?.Message, ex.Error?.Code);
        }

        return EmailResponse.Create("202", "The email was accepted for delivery.");
    }

    private static List<Recipient>? BuildRecipients(List<string>? emails)
    {
        if (emails == null || emails.Count == 0)
            return null;

        return [.. emails.Select(email => new Recipient
        {
            EmailAddress = new EmailAddress { Address = email }
        })];
    }

    private static List<Attachment>? BuildAttachments(List<string>? attachments)
    {
        if (attachments == null || attachments.Count == 0)
            return null;

        return [.. attachments.Select(attachment => new FileAttachment
        {
            Name = attachment,
            ContentType = "application/octet-stream",
            ContentBytes = Convert.FromBase64String(Convert.ToBase64String(File.ReadAllBytes(attachment)))
        }).Cast<Attachment>()];
    }

    public string BuildBody(string template, Dictionary<string, string> values)
    {
        if (string.IsNullOrEmpty(template))
            throw new ArgumentNullException(nameof(template));

        if (values == null || values.Count == 0)
            return template;

        foreach (var kvp in values)
        {
            template = template.Replace($"{{{{{kvp.Key}}}}}", kvp.Value);
        }

        return template;
    }
}
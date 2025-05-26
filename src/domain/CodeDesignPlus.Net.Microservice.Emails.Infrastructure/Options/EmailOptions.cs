using System;
using System.ComponentModel.DataAnnotations;

namespace CodeDesignPlus.Net.Microservice.Emails.Infrastructure.Options;

public class EmailOptions
{
    public const string Section = "Email";

    /// <summary>
    /// The tenant ID for the Azure Active Directory (AAD) application.
    /// </summary>
    [Required]
    public string TenantId { get; set; } = null!;
    /// <summary>
    /// The client ID for the application registered in Azure AD.
    /// </summary>
    [Required]
    public string ClientId { get; set; } = null!;
    /// <summary>
    /// The client secret for the application registered in Azure AD.
    /// </summary>
    [Required]
    public string ClientSecret { get; set; } = null!;
    /// <summary>
    /// The URL of the Graph API for the tenant.
    /// </summary>
    public string[] Scopes { get; set; } = ["https://graph.microsoft.com/.default"];
    /// <summary>
    /// The User ID of the user with a license to send emails.
    /// </summary>
    public string UserIdWithLicense { get; set; } = null!;

}

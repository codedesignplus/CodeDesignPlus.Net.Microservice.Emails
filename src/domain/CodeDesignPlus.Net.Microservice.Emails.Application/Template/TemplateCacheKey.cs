namespace CodeDesignPlus.Net.Microservice.Emails.Application.Template;

/// <summary>
/// The key under which <c>GET Template/{id}</c> keeps a template in the cache, shared by the query that fills it and the
/// commands that must empty it (pendings/674).
/// </summary>
/// <remarks>
/// It carries the owner of the template, its tenant or <c>global</c>: the query only looks for the session's tenant and the
/// global one, so a copy cached for one tenant is never served to another (as <c>GetEmailsById</c> since pendings/680).
/// </remarks>
public static class TemplateCacheKey
{
    /// <summary>
    /// The owner segment of a global template of the system.
    /// </summary>
    public const string Global = "global";

    /// <summary>
    /// The key of a template, by its owner.
    /// </summary>
    /// <param name="owner">The tenant that owns the template, or null for a global template.</param>
    /// <param name="id">The template.</param>
    /// <returns>The key.</returns>
    public static string For(Guid? owner, Guid id) => $"{(owner is { } tenant && tenant != Guid.Empty ? tenant.ToString() : Global)}:{id}";
}

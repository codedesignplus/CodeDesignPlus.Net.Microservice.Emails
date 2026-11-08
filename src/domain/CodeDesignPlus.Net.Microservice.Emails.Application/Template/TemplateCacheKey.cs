namespace CodeDesignPlus.Net.Microservice.Emails.Application.Template;

/// <summary>
/// The key under which <c>GET Template/{id}</c> keeps a template in the cache, shared by the query that fills it and the
/// commands that must empty it (pendings/674).
/// </summary>
public static class TemplateCacheKey
{
    /// <summary>
    /// The key of a template.
    /// </summary>
    /// <param name="id">The template.</param>
    /// <returns>The key.</returns>
    public static string For(Guid id) => id.ToString();
}

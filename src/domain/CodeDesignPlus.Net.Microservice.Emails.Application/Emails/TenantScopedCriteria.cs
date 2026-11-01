namespace CodeDesignPlus.Net.Microservice.Emails.Application.Emails;

/// <summary>
/// Acota el historial de correos a la copropiedad que lo pide (pendings/680).
/// </summary>
/// <remarks>
/// <see cref="EmailsAggregate"/> declara su propio <c>Tenant</c> y no hereda de <c>AggregateRoot</c>, así que el
/// repositorio no filtra solo por copropiedad: sin esto, cualquier administrador leía los correos de toda la plataforma.
/// El parser de criteria agrupa de izquierda a derecha y sin paréntesis, por eso la copropiedad va al final:
/// <c>x|or|y|and|tenant=A</c> se lee <c>(x o y) y copropiedad</c> y el filtro de la pantalla nunca sale de ella.
/// </remarks>
public static class TenantScopedCriteria
{
    /// <summary>
    /// Devuelve una copia de <paramref name="criteria"/> cuyo filtro solo deja pasar los correos de <paramref name="tenant"/>.
    /// </summary>
    public static C.Criteria Scope(Guid tenant, C.Criteria criteria)
    {
        ApplicationGuard.IsNull(criteria, Errors.InvalidRequest);

        var clientFilters = criteria.Filters?.Trim() ?? string.Empty;

        var tenantFilter = $"tenant={tenant}";

        return new C.Criteria
        {
            Filters = string.IsNullOrEmpty(clientFilters) ? tenantFilter : $"{clientFilters}|and|{tenantFilter}",
            OrderBy = criteria.OrderBy,
            OrderType = criteria.OrderType,
            Skip = criteria.Skip,
            Limit = criteria.Limit,
        };
    }
}

using System.Runtime.CompilerServices;

namespace CodeDesignPlus.Net.Microservice.Emails.Domain.Test;

/// <summary>
/// Una plantilla guardada antes del 680 no trae variables sensibles, y leerlas no puede fallar (pendings/680).
/// </summary>
/// <remarks>
/// El driver de Mongo crea el agregado sin pasar por los inicializadores. Con el campo ausente en el documento, la lista
/// quedaba nula y el 1-nov el envío de todos los correos se cayó al leerla.
/// </remarks>
public class SensitiveVariablesOfStoredTemplatesTest
{
    [Fact]
    public void SensitiveVariables_TemplateCreatedWithoutInitializers_IsEmpty()
    {
        // Arrange
        var template = (TemplateAggregate)RuntimeHelpers.GetUninitializedObject(typeof(TemplateAggregate));

        // Act & Assert
        Assert.Empty(template.SensitiveVariables);
    }
}

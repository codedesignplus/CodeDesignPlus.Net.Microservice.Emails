using System.Collections.Generic;

namespace CodeDesignPlus.Net.Microservice.Emails.Domain.Test;

/// <summary>
/// Las variables sensibles de una plantilla son algunas de sus variables, y se conservan al editarla (pendings/680).
/// </summary>
public class SensitiveVariablesTest
{
    private static readonly Guid UserId = Guid.NewGuid();

    private static TemplateAggregate Template(List<string>? sensitive) =>
        TemplateAggregate.Create(Guid.NewGuid(), "PasswordTemp", "Tu contraseña", "<p>{{password}}</p>", ["display_name", "password"], [], "no-reply@kappali.com", "Kappali", true, null, UserId, sensitive);

    [Fact]
    public void Create_SensitiveVariableNotAmongTheVariables_IsRejected()
    {
        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => Template(["code"]));

        // Assert
        Assert.Equal(Errors.SensitiveVariablesTemplateIsInvalid.Code, exception.Code);
    }

    [Fact]
    public void Update_WithoutSensitiveVariables_KeepsTheOnesStillDeclared()
    {
        // Arrange
        var template = Template(["password"]);

        // Act
        template.Update(template.Name, template.Subject, template.Body, template.Variables, [], template.From, template.Alias, template.IsHtml, UserId);

        // Assert
        Assert.Equal(["password"], template.SensitiveVariables);
    }

    [Fact]
    public void Update_VariableRemoved_DropsItFromTheSensitiveOnes()
    {
        // Arrange
        var template = Template(["password"]);

        // Act
        template.Update(template.Name, template.Subject, template.Body, ["display_name"], [], template.From, template.Alias, template.IsHtml, UserId);

        // Assert
        Assert.Empty(template.SensitiveVariables);
    }

    [Fact]
    public void DeclareSensitiveVariables_SeededTemplate_DeclaresThem()
    {
        // Arrange
        var template = Template(null);

        // Act
        template.DeclareSensitiveVariables(["password"], UserId);

        // Assert
        Assert.Equal(["password"], template.SensitiveVariables);
    }
}

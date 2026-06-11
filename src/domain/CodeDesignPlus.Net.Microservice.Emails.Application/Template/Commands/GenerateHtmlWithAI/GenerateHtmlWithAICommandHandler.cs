using CodeDesignPlus.Net.AI.Abstractions;
using CodeDesignPlus.Net.AI.Abstractions.Models;
using Microsoft.Extensions.Logging;

namespace CodeDesignPlus.Net.Microservice.Emails.Application.Template.Commands.GenerateHtmlWithAI;

public class GenerateHtmlWithAICommandHandler(
    IAIService aiService,
    ILogger<GenerateHtmlWithAICommandHandler> logger
) : IRequestHandler<GenerateHtmlWithAICommand, GenerateHtmlWithAIResult>
{
    private const string AgentName = "EmailDesigner";

    public async Task<GenerateHtmlWithAIResult> Handle(GenerateHtmlWithAICommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var context = BuildContext(request);

        var messages = new List<ChatMessage>
        {
            new(ChatRole.User, context)
        };

        try
        {
            var response = await aiService.ChatAsync(AgentName, messages, cancellationToken);

            if (!response.Success)
            {
                logger.LogWarning("AI generation failed: {Error}", response.Error);
                return new GenerateHtmlWithAIResult(request.CurrentHtml, response.Error ?? "Error generating HTML", false, response.Error);
            }

            var html = ExtractHtml(response.Content);

            return new GenerateHtmlWithAIResult(html, "Template actualizado exitosamente.", true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during AI HTML generation");
            return new GenerateHtmlWithAIResult(request.CurrentHtml, ex.Message, false, ex.Message);
        }
    }

    private static string BuildContext(GenerateHtmlWithAICommand request)
    {
        var variablesList = request.Variables.Count > 0
            ? string.Join(", ", request.Variables.Select(v => $"{{{{{v}}}}}"))
            : "ninguna";

        return $"""
            HTML actual del template:
            ```html
            {request.CurrentHtml}
            ```

            Variables disponibles para usar en el template: {variablesList}

            Instrucción del usuario: {request.Prompt}

            Responde SOLO con el HTML completo del template actualizado. No incluyas explicaciones ni markdown, solo el HTML puro.
            """;
    }

    private static string ExtractHtml(string content)
    {
        var html = content.Trim();

        if (html.StartsWith("```html", StringComparison.OrdinalIgnoreCase))
            html = html[7..];
        else if (html.StartsWith("```"))
            html = html[3..];

        if (html.EndsWith("```"))
            html = html[..^3];

        return html.Trim();
    }
}

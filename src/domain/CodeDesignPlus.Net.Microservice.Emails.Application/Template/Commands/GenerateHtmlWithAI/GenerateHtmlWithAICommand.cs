namespace CodeDesignPlus.Net.Microservice.Emails.Application.Template.Commands.GenerateHtmlWithAI;

[DtoGenerator]
public record GenerateHtmlWithAICommand(string Prompt, string CurrentHtml, List<string> Variables) : IRequest<GenerateHtmlWithAIResult>;

public record GenerateHtmlWithAIResult(string Html, string Message, bool Success, string? Error = null);

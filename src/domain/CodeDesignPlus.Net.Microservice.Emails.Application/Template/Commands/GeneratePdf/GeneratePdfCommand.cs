namespace CodeDesignPlus.Net.Microservice.Emails.Application.Template.Commands.GeneratePdf;

public record GeneratePdfCommand(string TemplateType, Dictionary<string, string> Values) : IRequest<GeneratePdfResult>;

public record GeneratePdfResult(byte[] PdfContent, bool Success, string Error);

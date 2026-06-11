namespace CodeDesignPlus.Net.Microservice.Emails.Application.Template.Commands.GeneratePdf;

public record GeneratePdfCommand(string TemplateType, Dictionary<string, string> Values) : IRequest<GeneratePdfResult>;

public record GeneratePdfResult(Guid Id, string Name, string Target, string SignedUrl, bool Success, string Error, byte[] PdfContent = null);

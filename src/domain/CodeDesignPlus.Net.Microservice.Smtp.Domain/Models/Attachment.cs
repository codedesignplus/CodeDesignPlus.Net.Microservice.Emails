using System;

namespace CodeDesignPlus.Net.Microservice.Smtp.Domain.Models;

public class Attachment
{
    public string Name { get; private set; } = string.Empty;

    public string ContentType { get; private set; } = string.Empty;

    public byte[] Content { get; private set; } = [];

    public long Size { get; private set; }

    public bool IsInline { get; private set; }

    private Attachment(string name, string contentType, byte[] content)
    {
        Name = name;
        Content = content;
        Size = content.Length;
        ContentType = contentType;
    }

    public static Attachment Create(string name, string contentType, byte[] content)
    {
        return new Attachment(name, contentType, content);
    }
}

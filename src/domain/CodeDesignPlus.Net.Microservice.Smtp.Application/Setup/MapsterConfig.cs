using System.ComponentModel.DataAnnotations;
using CodeDesignPlus.Microservice.Api.Dtos;
using CodeDesignPlus.Net.Microservice.Smtp.Application.Emails.Commands.SendEmail;
using CodeDesignPlus.Net.Microservice.Smtp.Application.Template.Commands.CreateTemplate;
using CodeDesignPlus.Net.Microservice.Smtp.Application.Template.Commands.UpdateTemplate;
using CodeDesignPlus.Net.Microservice.Smtp.Application.Template.DataTransferObjects;

namespace CodeDesignPlus.Net.Microservice.Smtp.Application.Setup;

public static class MapsterConfigEmails
{
    public static void Configure()
    {
        TypeAdapterConfig<CreateTemplateDto, CreateTemplateCommand>.NewConfig();
        TypeAdapterConfig<UpdateTemplateDto, UpdateTemplateCommand>.NewConfig();
        TypeAdapterConfig<TemplateAggregate, TemplateDto>.NewConfig();

        TypeAdapterConfig<TemplateAggregate, TemplateDto>.NewConfig();        
        
        TypeAdapterConfig<SendEmailDto, SendEmailCommand>.NewConfig();
        TypeAdapterConfig<EmailAddressAttribute, EmailsDto>.NewConfig();
    }
}

using System.ComponentModel.DataAnnotations;
using CodeDesignPlus.Microservice.Api.Dtos;
using CodeDesignPlus.Net.Microservice.Emails.Application.Template.Commands.CreateTemplate;
using CodeDesignPlus.Net.Microservice.Emails.Application.Template.Commands.UpdateTemplate;
using CodeDesignPlus.Net.Microservice.Emails.Application.Template.DataTransferObjects;

namespace CodeDesignPlus.Net.Microservice.Emails.Application.Setup;

public static class MapsterConfigEmails
{
    public static void Configure()
    {
        TypeAdapterConfig<CreateTemplateDto, CreateTemplateCommand>.NewConfig();
        TypeAdapterConfig<UpdateTemplateDto, UpdateTemplateCommand>.NewConfig();
        TypeAdapterConfig<TemplateAggregate, TemplateDto>.NewConfig();
        TypeAdapterConfig<EmailAddressAttribute, EmailsDto>.NewConfig();
    }
}

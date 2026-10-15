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

        // El detalle lleva el nombre de cada adjunto, como el evento de envio; sin esto salia el nombre de la clase.
        TypeAdapterConfig<EmailsAggregate, EmailsDto>.NewConfig()
            .Map(dest => dest.Attachments, src => src.Attachments.Select(x => x.Name).ToList());
    }
}

using System.ComponentModel.DataAnnotations;
using CodeDesignPlus.Microservice.Api.Dtos;
using CodeDesignPlus.Net.Microservice.Emails.Application.Emails.Commands.SendEmail;
using CodeDesignPlus.Net.Microservice.Emails.Application.Template.Commands.CreateTemplate;
using CodeDesignPlus.Net.Microservice.Emails.Application.Template.Commands.UpdateTemplate;
using CodeDesignPlus.Net.Microservice.Emails.Application.Template.DataTransferObjects;
using CodeDesignPlus.Net.Microservice.Emails.Application.User.Commands.CreateConfigUserTemplate;
using CodeDesignPlus.Net.Microservice.Emails.Application.User.Commands.SendMailPasswordTemp;
using CodeDesignPlus.Net.Microservice.Emails.Application.User.Commands.UpdateConfigUserTemplate;
using CodeDesignPlus.Net.Microservice.Emails.Domain.Models;
using Microsoft.AspNetCore.Http;

namespace CodeDesignPlus.Net.Microservice.Emails.Application.Setup;

public static class MapsterConfigEmails
{
    public static void Configure()
    {
        TypeAdapterConfig<CreateTemplateDto, CreateTemplateCommand>.NewConfig();
        TypeAdapterConfig<UpdateTemplateDto, UpdateTemplateCommand>.NewConfig();
        TypeAdapterConfig<TemplateAggregate, TemplateDto>.NewConfig();

        TypeAdapterConfig<TemplateAggregate, TemplateDto>.NewConfig();

        TypeAdapterConfig<EmailAddressAttribute, EmailsDto>.NewConfig();


        TypeAdapterConfig<CreateConfigUserTemplateDto, CreateConfigUserTemplateCommand>.NewConfig();
        TypeAdapterConfig<UpdateConfigUserTemplateDto, UpdateConfigUserTemplateCommand>.NewConfig();
    }


}

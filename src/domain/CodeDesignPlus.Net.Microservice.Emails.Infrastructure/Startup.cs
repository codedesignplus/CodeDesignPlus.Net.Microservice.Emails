using CodeDesignPlus.Net.AI.Extensions;
using CodeDesignPlus.Net.File.Storage.Extensions;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Infrastructure.Services.GraphClient;
using CodeDesignPlus.Net.Microservice.Emails.Domain.Services;
using CodeDesignPlus.Net.Microservice.Emails.Infrastructure.Options;
using CodeDesignPlus.Net.Microservice.Emails.Infrastructure.Seeds;
using CodeDesignPlus.Net.Microservice.Emails.Infrastructure.Services.EmailSender;
using CodeDesignPlus.Net.Microservice.Emails.Infrastructure.Services.PdfGenerator;

namespace CodeDesignPlus.Net.Microservice.Emails.Infrastructure
{
    public class Startup : IStartup
    {
        public void Initialize(IServiceCollection services, IConfiguration configuration)
        {
            var section = configuration.GetRequiredSection(EmailOptions.Section);

            services.AddOptions<EmailOptions>()
                .Bind(section)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddScoped<IGraphClient, GraphClient>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<IPdfGenerator, PdfGenerator>();
            services.AddFileStorage(configuration);
            services.AddAI(configuration);
            services.AddHostedService<SystemTemplateSeedService>();
        }
    }
}

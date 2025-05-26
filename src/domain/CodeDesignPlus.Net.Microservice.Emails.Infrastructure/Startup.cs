using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Infrastructure.Services.GraphClient;
using CodeDesignPlus.Net.Microservice.Emails.Domain.Services;
using CodeDesignPlus.Net.Microservice.Emails.Infrastructure.Options;
using CodeDesignPlus.Net.Microservice.Emails.Infrastructure.Services.EmailSender;

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
        }
    }
}

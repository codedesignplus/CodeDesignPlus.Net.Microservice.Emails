using CodeDesignPlus.Net.Microservice.Smtp.Domain.Services;
using CodeDesignPlus.Net.Microservice.Smtp.Infrastructure.Options;
using CodeDesignPlus.Net.Microservice.Smtp.Infrastructure.Services.EmailSender;

namespace CodeDesignPlus.Net.Microservice.Smtp.Infrastructure
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

            services.AddScoped<IEmailSender, EmailSender>();
        }
    }
}

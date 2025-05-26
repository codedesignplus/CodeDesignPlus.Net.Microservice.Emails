namespace CodeDesignPlus.Net.Microservice.Emails.Infrastructure.Repositories;

public class TemplateRepository(IServiceProvider serviceProvider, IOptions<MongoOptions> mongoOptions, ILogger<TemplateRepository> logger) 
    : RepositoryBase(serviceProvider, mongoOptions, logger), ITemplateRepository
{
   
}
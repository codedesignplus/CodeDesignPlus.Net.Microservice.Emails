namespace CodeDesignPlus.Net.Microservice.Smtp.Infrastructure.Repositories;

public class TemplateRepository(IServiceProvider serviceProvider, IOptions<MongoOptions> mongoOptions, ILogger<TemplateRepository> logger) 
    : RepositoryBase(serviceProvider, mongoOptions, logger), ITemplateRepository
{
   
}
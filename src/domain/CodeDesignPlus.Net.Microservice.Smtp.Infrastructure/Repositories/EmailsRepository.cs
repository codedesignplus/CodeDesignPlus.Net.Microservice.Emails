namespace CodeDesignPlus.Net.Microservice.Smtp.Infrastructure.Repositories;

public class EmailsRepository(IServiceProvider serviceProvider, IOptions<MongoOptions> mongoOptions, ILogger<EmailsRepository> logger) 
    : RepositoryBase(serviceProvider, mongoOptions, logger), IEmailsRepository
{
   
}
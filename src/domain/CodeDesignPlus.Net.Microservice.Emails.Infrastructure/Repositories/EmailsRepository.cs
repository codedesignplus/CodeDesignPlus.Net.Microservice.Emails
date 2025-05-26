namespace CodeDesignPlus.Net.Microservice.Emails.Infrastructure.Repositories;

public class EmailsRepository(IServiceProvider serviceProvider, IOptions<MongoOptions> mongoOptions, ILogger<EmailsRepository> logger) 
    : RepositoryBase(serviceProvider, mongoOptions, logger), IEmailsRepository
{
   
}
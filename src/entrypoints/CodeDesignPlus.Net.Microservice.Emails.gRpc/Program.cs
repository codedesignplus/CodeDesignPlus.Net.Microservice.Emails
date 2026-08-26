using CodeDesignPlus.Net.Microservice.Emails.Infrastructure.Services.PdfGenerator;
using CodeDesignPlus.Net.Observability.Interceptors;
using CodeDesignPlus.Net.gRpc.Clients.Extensions;
using CodeDesignPlus.Net.Logger.Extensions;
using CodeDesignPlus.Net.Microservice.Commons.EntryPoints.gRpc.Interceptors;
using CodeDesignPlus.Net.Microservice.Commons.FluentValidation;
using CodeDesignPlus.Net.Microservice.Commons.HealthChecks;
using CodeDesignPlus.Net.Microservice.Commons.MediatR;
using CodeDesignPlus.Net.Microservice.Emails.gRpc.Services;
using CodeDesignPlus.Net.Mongo.Extensions;
using CodeDesignPlus.Net.Observability.Extensions;
using CodeDesignPlus.Net.RabbitMQ.Extensions;
using CodeDesignPlus.Net.Redis.Cache.Extensions;
using CodeDesignPlus.Net.Redis.Extensions;
using CodeDesignPlus.Net.Security.Extensions;
using CodeDesignPlus.Net.Vault.Extensions;

var builder = WebApplication.CreateSlimBuilder(args);

Serilog.Debugging.SelfLog.Enable(Console.Error);

builder.Host.UseSerilog();

builder.Configuration.AddVault();

builder.Services.AddGrpc(options =>
{
    options.Interceptors.Add<ErrorInterceptor>();
    options.Interceptors.Add<TraceContextInterceptor>();
});
builder.Services.AddGrpcReflection();

builder.Services.AddVault(builder.Configuration);
builder.Services.AddMapster();
builder.Services.AddMediatR<CodeDesignPlus.Net.Microservice.Emails.Application.Startup>();
builder.Services.AddFluentValidation();

builder.Services.AddMongo<CodeDesignPlus.Net.Microservice.Emails.Infrastructure.Startup>(builder.Configuration);
builder.Services.AddRedis(builder.Configuration);
builder.Services.AddRabbitMQ<CodeDesignPlus.Net.Microservice.Emails.Domain.Startup>(builder.Configuration);
builder.Services.AddSecurity(builder.Configuration);
// EL MEDIDOR DEL PDF SE DECLARA AQUI O NO SALE DEL PROCESO. OpenTelemetry solo exporta los
// Meter que se le nombran: sin esta linea las metricas se emiten y nadie las recoge, que es
// justo como no tenerlas. Son las que dicen cuantos renderizados hay a la vez, cuanto esperan
// en cola y cuantos se descartan por no conseguir hueco.
builder.Services.AddObservability(builder.Configuration, builder.Environment,
    metrics => metrics.AddMeter(PdfGenerator.MeterName));
builder.Services.AddLogger(builder.Configuration);
builder.Services.AddCache(builder.Configuration);
builder.Services.AddHealthChecksServices();
builder.Services.AddGrpcClients(builder.Configuration);

var app = builder.Build();

app.UseHealthChecks();

app.UseAuth();

app.MapGrpcService<EmailsService>();//.RequireAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService();
}

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

await app.RunAsync();

public partial class Program
{
    protected Program() { }
}
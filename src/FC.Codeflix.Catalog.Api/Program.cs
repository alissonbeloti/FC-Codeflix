using FC.Codeflix.Catalog.Api.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAppConnections(builder.Configuration)
    .AddUseCases()
    .AddRabbitMQ(builder.Configuration)
    .AddMessageProducer()
    .AddMessageConsumer()
    .AddStorage(builder.Configuration)
    .AddConfigurationsControllers();

var app = builder.Build();
app.UseDocumentation();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }
using MediatR;
using FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;
using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FC.Codeflix.Catalog.Infra.Data.EF;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using FC.Codeflix.Catalog.Domain.SeedWork;
using FC.Codeflix.Catalog.Application;
using FC.Codeflix.Catalog.Domain.Events;
using FC.Codeflix.Catalog.Application.EventHandlers;
using FC.CodeFlix.Catalog.Infra.Message.Configuration;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using FC.CodeFlix.Catalog.Infra.Message.Producer;

namespace FC.Codeflix.Catalog.Api.Configurations;

public static class UseCasesConfiguration
{
    public static IServiceCollection AddUseCases(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateCategory).Assembly));
        services.AddRepositories();
        services.AddDomainEvents(configuration);
        return services; 
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddTransient<ICategoryRepository, CategoryRepository>();
        services.AddTransient<IGenreRepository, GenreRepository>();
        services.AddTransient<ICastMemberRepository, CastMemberRepository>();
        services.AddTransient<IVideoRepository, VideoRepository>();
        services.AddTransient<IUnitOfWork, UnitOfWork>();

        return services;
    }

    private static IServiceCollection AddDomainEvents(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddTransient<IDomainEventPublisher, DomainEventPublisher>();
        services.AddTransient<IDomainEventHandler<VideoUploadedEvent>, 
            SendToEncoderEventHandler>();

        services.Configure<RabbitMQConfiguration>(
            configuration.GetSection(RabbitMQConfiguration.ConfigurationSection));
        services.AddSingleton(sp =>
        {
            var config = sp.GetRequiredService<IOptions<RabbitMQConfiguration>>().Value;
            var factory = new ConnectionFactory
            {
                HostName = config.Hostname,
                UserName = config.Username,
                Password = config.Password,
                Port = config.Port!.Value,
            };
            return factory.CreateConnection();
        });
        services.AddSingleton<ChannelManager>();
        services.AddTransient<IMessageProducer>(sp =>
        {
            var channelManager = sp.GetRequiredService<ChannelManager>();
            var config = sp.GetRequiredService<IOptions<RabbitMQConfiguration>>();
            return new RabbitMQProducer(channelManager.GetChannel(), config);
        });


        return services;
    }
}

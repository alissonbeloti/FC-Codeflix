﻿using FC.Codeflix.Catalog.Api.Filters;
using FC.CodeFlix.Catalog.Infra.Message.JsonPolicies;

namespace FC.Codeflix.Catalog.Api.Configurations;

public static class ControllersConfiguration
{
    public static IServiceCollection AddConfigurationsControllers(this IServiceCollection services)
    {
        services
            .AddControllers(
            opt => opt.Filters.Add(typeof(ApiGlobalExceptionFilter))
            )
            .AddJsonOptions(jsonOptions => {
                jsonOptions.JsonSerializerOptions.PropertyNamingPolicy
                    = new JsonSnakeCasePolicy();
            });
            ;
        services.AddDocumentation();

        return services;
    }

    public static IServiceCollection AddDocumentation(this IServiceCollection services)
    {
        
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }


    public static WebApplication UseDocumentation(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        return app;
    }

    
}

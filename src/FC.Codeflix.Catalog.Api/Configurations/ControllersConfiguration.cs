using FC.Codeflix.Catalog.Api.Filters;
using FC.CodeFlix.Catalog.Infra.Message.JsonPolicies;

using Microsoft.OpenApi.Models;

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
        services.AddSwaggerGen(option =>
        {
            option.SwaggerDoc("v1", new OpenApiInfo { Title = "FC3 Codeflix Catalog", Version = "v1" });
            option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });
            option.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type=ReferenceType.SecurityScheme,
                            Id="Bearer"
                        }
                    },
                    new string[]{}
                }
            });
        });
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

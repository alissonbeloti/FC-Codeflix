﻿using MediatR;
using FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;
using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FC.Codeflix.Catalog.Infra.Data.EF;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace FC.Codeflix.Catalog.Api.Configurations;

public static class UseCasesConfiguration
{
    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateCategory).Assembly));
        services.AddRepositories();

        return services; 
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddTransient<ICategoryRepository, CategoryRepository>();
        services.AddTransient<IGenreRepository, GenreRepository>();
        services.AddTransient<IUnitOfWork, UnitOfWork>();

        return services;
    }
}

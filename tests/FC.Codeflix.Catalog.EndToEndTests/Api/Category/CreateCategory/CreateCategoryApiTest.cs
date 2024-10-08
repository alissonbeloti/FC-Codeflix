﻿using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;
using FC.Codeflix.Catalog.Api.ApiModels.Response;


namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.CreateCategory;
[Collection(nameof(CreateCategoryApiTestFixture))]
public class CreateCategoryApiTest :
    IDisposable
{
    private readonly CreateCategoryApiTestFixture _fixture;

    public CreateCategoryApiTest(CreateCategoryApiTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(CreateCategory))]
    [Trait("EndToEnd/API", "Category/Create - Endpoints")]
    public async Task CreateCategory()
    {
        var input = _fixture.GetExampleInput();

        var (response, output) = await _fixture.ApiClient.Post<ApiResponse<CategoryModelOutput>>(
            "/categories",
            input
            );
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        output.Should().NotBeNull();
        output.Data.Id.Should().NotBeEmpty();
        output.Data.Name.Should().Be(input.Name);
        output.Data.Description.Should().Be(input.Description);
        output.Data.IsActive.Should().Be(input.IsActive);
        output.Data.CreatedAt.Should().NotBeSameDateAs(default(DateTime));

        DomainEntity.Category? dbCategory = await _fixture.Persistence
            .GetById(output.Data.Id);
        dbCategory.Should().NotBeNull();
        dbCategory.Id.Should().NotBeEmpty();
        dbCategory.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be(input.Description);
        dbCategory.IsActive.Should().Be(input.IsActive);
        dbCategory.CreatedAt.Should().NotBeSameDateAs(default);
    }

    public void Dispose()
        => _fixture.CleanPersistence();

    [Theory(DisplayName = nameof(ErrorWhenCanInstantiateAggregate))]
    [Trait("EndToEnd/API", "Category/Create - Endpoints")]
    [MemberData(nameof(CreateCategoryApiTestDataGenerator.GetInvalidNamesAndDescription),
        MemberType = typeof(CreateCategoryApiTestDataGenerator))]
    public async Task ErrorWhenCanInstantiateAggregate(CreateCategoryInput input, string expectedDetail)
    {
        var (response, output) = await _fixture.ApiClient.Post<ProblemDetails>(
            "/categories",
            input
            );
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        output.Should().NotBeNull();
        output!.Title.Should().Be("One or more validation errors occurred");
        output.Type.Should().Be("UnprocessableEntity");
        output.Status.Should().Be((int)HttpStatusCode.UnprocessableEntity);
        output.Detail.Should().Be(expectedDetail);

    }
}

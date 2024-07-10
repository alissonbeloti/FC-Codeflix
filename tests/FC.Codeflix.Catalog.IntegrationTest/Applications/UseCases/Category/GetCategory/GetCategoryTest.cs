﻿using UseCase = FC.Codeflix.Catalog.Application.UseCases.Category.GetCategory;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FluentAssertions;
using FC.Codeflix.Catalog.Application.Exceptions;

namespace FC.Codeflix.Catalog.IntegrationTest.Applications.UseCases.Category.GetCategory;
[Collection(nameof(GetCategoryTestFixture))]
public class GetCategoryTest
{
    private readonly GetCategoryTestFixture _fixture;

    public GetCategoryTest(GetCategoryTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(GetCategory))]
    [Trait("Integration/Application", "GetCategory - Use Cases")]
    public async Task GetCategory()
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleCategory = _fixture.GetExampleCategory();
        dbContext.Categories.Add(exampleCategory);
        await dbContext.SaveChangesAsync();
        var repository = new CategoryRepository(dbContext);

        var input = new UseCase.GetCategoryInput(exampleCategory.Id);
        var useCase = new UseCase.GetCategory(repository);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Name.Should().Be(exampleCategory.Name);
        output.Description.Should().Be(exampleCategory.Description);
        output.IsActive.Should().Be(exampleCategory.IsActive);
        output.Id.Should().Be(exampleCategory.Id);
        output.CreatedAt.Should().Be(exampleCategory.CreatedAt);
    }

    [Fact(DisplayName = nameof(GetCategoryNotFoundExceptionWhenCategoryDoesntExists))]
    [Trait("Integration/Application", "GetCategory - Use Cases")]
    public async Task GetCategoryNotFoundExceptionWhenCategoryDoesntExists()
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleCategory = _fixture.GetExampleCategory();
        dbContext.Categories.Add(exampleCategory);
        await dbContext.SaveChangesAsync();
        var repository = new CategoryRepository(dbContext);
        var idFind = Guid.NewGuid();
        var input = new UseCase.GetCategoryInput(idFind);
        var useCase = new UseCase.GetCategory(repository);

        var task = async () =>
            await useCase.Handle(input, CancellationToken.None);

        await task.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Category '{idFind}' not found.");
    }
}

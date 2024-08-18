using FC.Codeflix.Catalog.Application;
using FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FluentAssertions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using ApplicationUseCases = FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;

namespace FC.Codeflix.Catalog.IntegrationTest.Applications.UseCases.Category.CreateCategory;

[Collection(nameof(CreateCategoryTestFixture))]
public class CreateCategoryTest
{
    private readonly CreateCategoryTestFixture _fixture;

    public CreateCategoryTest(CreateCategoryTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(CreateCategoryOK))]
    [Trait("Integration/Application", "Use Cases - CreateCategory")]
    private async Task CreateCategoryOK()
    {
        var dbContext = _fixture.CreateDbContext();
        var repository = new CategoryRepository(dbContext);
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventPublisher = new DomainEventPublisher(serviceProvider);
        var unitOfWork = new UnitOfWork(dbContext, eventPublisher,
            serviceProvider.GetRequiredService<ILogger<UnitOfWork>>());
        var useCase = new ApplicationUseCases.CreateCategory(repository, unitOfWork);
        var input = _fixture.GetInput();

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();

        var dbCategory = await (_fixture.CreateDbContext(true)).Categories.FindAsync(output.Id);

        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be(input?.Description);
        dbCategory.IsActive.Should().Be(input!.IsActive);
        dbCategory.CreatedAt.Should().Be(output.CreatedAt);

        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be(input.IsActive);
        output.Id.Should().NotBeEmpty();
        (output.CreatedAt != default).Should().BeTrue();
    }

    [Fact(DisplayName = nameof(CreateCategoryOnlyWithName))]
    [Trait("Integration/Application", "Use Cases - CreateCategory")]
    public async Task CreateCategoryOnlyWithName()
    {

        var dbContext = _fixture.CreateDbContext();
        var repository = new CategoryRepository(dbContext);
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventPublisher = new DomainEventPublisher(serviceProvider);
        var unitOfWork = new UnitOfWork(dbContext, eventPublisher,
            serviceProvider.GetRequiredService<ILogger<UnitOfWork>>());
        var useCase = new ApplicationUseCases.CreateCategory(repository, unitOfWork);

        var input = new CreateCategoryInput(_fixture.GetValidCategoryName());

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Description.Should().BeEmpty();
        output.IsActive.Should().BeTrue();
        output.Id.Should().NotBeEmpty();
        (output.CreatedAt != default).Should().BeTrue();

        var dbCategory = await (_fixture.CreateDbContext(true)).Categories.FindAsync(output.Id);

        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be(input?.Description);
        dbCategory.IsActive.Should().Be(input!.IsActive);
        dbCategory.CreatedAt.Should().Be(output.CreatedAt);
    }

    [Fact(DisplayName = nameof(CreateCategoryOnlyWithNameAndDescription))]
    [Trait("Integration/Application", "Use Cases - CreateCategory")]
    public async Task CreateCategoryOnlyWithNameAndDescription()
    {

        var dbContext = _fixture.CreateDbContext();
        var repository = new CategoryRepository(dbContext);
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventPublisher = new DomainEventPublisher(serviceProvider);
        var unitOfWork = new UnitOfWork(dbContext, eventPublisher,
            serviceProvider.GetRequiredService<ILogger<UnitOfWork>>());
        var useCase = new ApplicationUseCases.CreateCategory(repository, unitOfWork);

        var input = new CreateCategoryInput(
            _fixture.GetValidCategoryName(), 
            _fixture.GetValidCategoryDescription());

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().BeTrue();
        output.Id.Should().NotBeEmpty();
        (output.CreatedAt != default).Should().BeTrue();

        var dbCategory = await (_fixture.CreateDbContext(true)).Categories.FindAsync(output.Id);

        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be(input?.Description);
        dbCategory.IsActive.Should().Be(input!.IsActive);
        dbCategory.CreatedAt.Should().Be(output.CreatedAt);
    }

    [Theory(DisplayName = nameof(ThrowWhenCantInstantiateCategory))]
    [Trait("Integration/Application", "Use Cases - CreateCategory")]
    [MemberData(nameof(CreateCategoryTestDataGenerator.GetInvalidInputs),
        parameters: 6,
        MemberType = typeof(CreateCategoryTestDataGenerator))]
    public async Task ThrowWhenCantInstantiateCategory(CreateCategoryInput input, string expectedExceptionMessage)
    {

        var dbContext = _fixture.CreateDbContext();
        var repository = new CategoryRepository(dbContext);
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventPublisher = new DomainEventPublisher(serviceProvider);
        var unitOfWork = new UnitOfWork(dbContext, eventPublisher,
            serviceProvider.GetRequiredService<ILogger<UnitOfWork>>());
        var useCase = new ApplicationUseCases.CreateCategory(repository, unitOfWork);

        var taks = async () => await useCase.Handle(input, CancellationToken.None);

        await taks.Should().ThrowAsync<EntityValidationException>()
            .WithMessage(expectedExceptionMessage);

        var dbCategoriesList = _fixture.CreateDbContext(true)
            .Categories.AsNoTracking().ToList();
        dbCategoriesList.Should().HaveCount(0);
    }
}

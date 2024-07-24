using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

using Moq;
using FC.Codeflix.Catalog.Application.UseCases.Genre.CreateGenre;
using FluentAssertions;
using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.Exceptions;

namespace FC.Codeflix.Catalog.UnitTests.Application.Genre.Create;

[Collection(nameof(CreateGenreTestFixture))]
public class CreateGenreTest
{
    private readonly CreateGenreTestFixture _fixture;

    public CreateGenreTest(CreateGenreTestFixture fixture) => _fixture = fixture;

    [Fact(DisplayName = nameof(CreateGenre))]
    [Trait("Application", "Genre - Use Cases")]
    public async Task CreateGenre()
    {

        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var categoryRepositoryMock = _fixture.GetCategoryRepositoryMock();
        var useCase = new CreateGenre(repositoryMock.Object, unitOfWorkMock.Object, categoryRepositoryMock.Object);
        var input = _fixture.GetExampleInput();

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        repositoryMock.Verify(
            repository => repository.Insert(It.IsAny<DomainEntity.Genre>(), It.IsAny<CancellationToken>()),
            Times.Once);
        unitOfWorkMock.Verify(
            uow => uow.Commit(It.IsAny<CancellationToken>()),
            Times.Once);
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be(input.IsActive);
        output.Categories.Should().HaveCount(0);
        output.Id.Should().NotBeEmpty();
        (output.CreatedAt != default).Should().BeTrue();
    }

    [Fact(DisplayName = nameof(CreateGenreWithRelatedCategories))]
    [Trait("Application", "Genre - Use Cases")]
    public async Task CreateGenreWithRelatedCategories()
    {

        var input = _fixture.GetExampleInputWithCategories();
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var categoryRepositoryMock = _fixture.GetCategoryRepositoryMock();
        categoryRepositoryMock.Setup(x => x.GetIdsListByIds(
           It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()
           )
       ).ReturnsAsync(input.CategoriesIds!);

        var useCase = new CreateGenre(repositoryMock.Object, unitOfWorkMock.Object,
            categoryRepositoryMock.Object);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        repositoryMock.Verify(
            repository => repository.Insert(It.IsAny<DomainEntity.Genre>(), It.IsAny<CancellationToken>()),
            Times.Once);
        unitOfWorkMock.Verify(
            uow => uow.Commit(It.IsAny<CancellationToken>()),
            Times.Once);
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be(input.IsActive);
        output.Categories.Should().HaveCount(input.CategoriesIds?.Count ?? 0);
        input.CategoriesIds?.ForEach(id => output.Categories.Should()
            .Contain(relation => relation.Id == id));
        output.Id.Should().NotBeEmpty();
        (output.CreatedAt != default).Should().BeTrue();
    }

    [Fact(DisplayName = nameof(CreateGenreThrowWhenRelatedCategoryNotFound))]
    [Trait("Application", "Genre - Use Cases")]
    public async Task CreateGenreThrowWhenRelatedCategoryNotFound()
    {
        var input = _fixture.GetExampleInputWithCategories();
        var exampleGuid = input.CategoriesIds![^1];
        var repositoryMock = _fixture.GetRepositoryMock();
        var categoryRepositoryMock = _fixture.GetCategoryRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        categoryRepositoryMock.Setup(x => x.GetIdsListByIds(
            It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(input.CategoriesIds.FindAll(x => x != exampleGuid));

        var useCase = new CreateGenre(repositoryMock.Object, unitOfWorkMock.Object,
            categoryRepositoryMock.Object);

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<RelatedAggregateException>()
            .WithMessage($"Related category id (or ids) not found: {exampleGuid}.");
        categoryRepositoryMock.Verify(x => x.GetIdsListByIds(
            It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()
            ), Times.Once);
    }

    [Theory(DisplayName = nameof(ThrowWhenNameIsEmptyOrNull))]
    [Trait("Domain", "Genre - Aggregates")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public async Task ThrowWhenNameIsEmptyOrNull(string? name)
    {
        var input = _fixture.GetExampleInput();
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var categoryRepositoryMock = _fixture.GetCategoryRepositoryMock();

        input.Name = name;
        var useCase = new CreateGenre(repositoryMock.Object, unitOfWorkMock.Object,
            categoryRepositoryMock.Object);

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<EntityValidationException>()
            .WithMessage<EntityValidationException>("Name should not be empty or null");
        
    }
}

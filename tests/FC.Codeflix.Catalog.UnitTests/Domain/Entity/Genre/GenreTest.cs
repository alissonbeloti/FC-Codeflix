using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Exceptions;

using FluentAssertions;

using System.Text.RegularExpressions;

using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.Genre;

[Collection(nameof(GenreTestFixture))]
public class GenreTest
{
    private readonly GenreTestFixture _fixture;

    public GenreTest(GenreTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "Genre - Aggregates")]
    public void Instantiate()
    {
        var genreName = "Horror";

        var datetimeBefore = DateTime.Now;
        var genre = new DomainEntity.Genre(genreName);
        var datetimeAfter = DateTime.Now.AddMicroseconds(1);

        genre.Should().NotBeNull();
        genre.Name.Should().Be(genreName);
        genre.IsActive.Should().BeTrue();   
        genre.CreatedAt.Should().NotBeSameDateAs(default);
        (genre.CreatedAt >= datetimeBefore).Should().BeTrue();
        (genre.CreatedAt <= datetimeAfter).Should().BeTrue();
    }

    [Theory(DisplayName = nameof(InstantiateWithIsActive))]
    [Trait("Domain", "Genre - Aggregates")]
    [InlineData(true)]
    [InlineData(false)]
    public void InstantiateWithIsActive(bool isActive)
    {
        var validGenre = _fixture.GetValidGenre();

        var datetimeBefore = DateTime.Now;

        var genre = new DomainEntity.Genre(validGenre.Name, isActive);

        var datetimeAfter = DateTime.Now.AddMicroseconds(1);

        genre.Should().NotBeNull();
        genre.Name.Should().Be(validGenre.Name);
        genre.Id.Should().NotBeEmpty();
        genre.CreatedAt.Should().NotBeSameDateAs(default(DateTime));
        (genre.CreatedAt > datetimeBefore).Should().BeTrue();
        (genre.CreatedAt < datetimeAfter).Should().BeTrue();
        genre.IsActive.Should().Be(isActive);
    }

    [Theory(DisplayName = nameof(Activate))]
    [Trait("Domain", "Genre - Aggregates")]
    [InlineData(true)]
    [InlineData(false)]
    public void Activate(bool isActive)
    {
        var validGenre = _fixture.GetValidGenre();

        var genre = new DomainEntity.Genre(validGenre.Name, isActive);

        genre.Activate();

        genre.Should().NotBeNull();
        genre.Name.Should().Be(validGenre.Name);
        genre.Id.Should().NotBeEmpty();
        genre.CreatedAt.Should().NotBeSameDateAs(default(DateTime));
        genre.IsActive.Should().BeTrue();
    }

    [Theory(DisplayName = nameof(Deactivate))]
    [Trait("Domain", "Genre - Aggregates")]
    [InlineData(true)]
    [InlineData(false)]
    public void Deactivate(bool isActive)
    {
        var validGenre = _fixture.GetValidGenre();

        var genre = new DomainEntity.Genre(validGenre.Name, isActive);

        genre.Deactivate();

        genre.Should().NotBeNull();
        genre.CreatedAt.Should().NotBeSameDateAs(default(DateTime));
        genre.IsActive.Should().BeFalse();
    }

    [Fact(DisplayName = "Update")]
    [Trait("Domain", "Genre - Aggregates")]
    public void Update()
    {
        var validGenre = _fixture.GetValidGenre();
        var genre = new DomainEntity.Genre(validGenre.Name, validGenre.IsActive);
        var newValues = _fixture.GetValidGenre();

        genre.Update(newValues.Name!);
        genre.Id.Should().NotBeEmpty();
        genre.Name.Should().Be(newValues.Name);
    }

    [Fact(DisplayName = nameof(UpdateOnlyName))]
    [Trait("Domain", "Genre - Aggregates")]
    public void UpdateOnlyName()
    {
        var genre = _fixture.GetValidGenre();
        var newValues = new { Name = _fixture.GetValidName(), };
        genre.Update(newValues.Name);
        genre.Name.Should().Be(newValues.Name);

    }

    [Theory(DisplayName = nameof(UpdateErrorWhenNameIsEmpty))]
    [Trait("Domain", "Genre - Aggregates")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void UpdateErrorWhenNameIsEmpty(string? name)
    {
        var genre = _fixture.GetValidGenre();
        Action action = () => genre.Update(name!);
        action.Should().Throw<EntityValidationException>()
            .WithMessage("Name should not be empty or null");
    }
           
    [Fact(DisplayName = nameof(AddCategory))]
    [Trait("Domain", "Genre - Aggregates")]
    public void AddCategory()
    {
        var genre = _fixture.GetValidGenre();
        var categoryGuid = Guid.NewGuid();

        genre.AddCategory(categoryGuid);

        genre.Categories.Should().HaveCount(1);
        genre.Categories.Should().Contain(categoryGuid);
    }

    [Fact(DisplayName = nameof(AddCategory))]
    [Trait("Domain", "Genre - Aggregates")]
    public void AddTwoCategory()
    {
        var genre = _fixture.GetValidGenre();
        var categoryGuid = Guid.NewGuid();
        var categoryGuid2 = Guid.NewGuid();

        genre.AddCategory(categoryGuid);
        genre.AddCategory(categoryGuid2);

        genre.Categories.Should().HaveCount(2);
        genre.Categories.Should().Contain(categoryGuid);
        genre.Categories.Should().Contain(categoryGuid2);
    }

    [Fact(DisplayName = nameof(RemoveCategory))]
    [Trait("Domain", "Genre - Aggregates")]
    public void RemoveCategory()
    {
        var exampleGuid = Guid.NewGuid();
        var genre = _fixture.GetValidGenre(categoriesList: new List<Guid>
        {
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), exampleGuid
        });

        genre .RemoveCategory(exampleGuid);

        genre.Categories.Should().HaveCount(4);
        genre.Categories.Should().NotContain(exampleGuid);
    }

    [Fact(DisplayName = nameof(RemoveAllCategory))]
    [Trait("Domain", "Genre - Aggregates")]
    public void RemoveAllCategory()
    {
        var exampleGuid = Guid.NewGuid();
        var genre = _fixture.GetValidGenre(categoriesList: new List<Guid>
        {
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), exampleGuid
        });

        genre.RemoveAllCategories();
        genre.Categories.Should().HaveCount(0);
    }
}

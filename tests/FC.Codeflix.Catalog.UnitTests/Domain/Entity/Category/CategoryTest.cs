using FC.Codeflix.Catalog.Domain.Exceptions;

using FluentAssertions;

using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.Category;

[Collection(nameof(CategoryTestFixture))]
public class CategoryTest
{
    private readonly CategoryTestFixture _categoryTestFixture;

    public CategoryTest(CategoryTestFixture categoryTestFixture)
        => _categoryTestFixture = categoryTestFixture;
    

    [Fact(DisplayName = "Instantiate")]
    [Trait("Domain", "Category - Aggregates")]
    public void Instantiate()
    {
        var validCategory = _categoryTestFixture.GetValidCategory();

        var datetimeBefore = DateTime.Now;

        var category = new DomainEntity.Category(validCategory.Name, validCategory.Description);

        var datetimeAfter = DateTime.Now.AddMicroseconds(1);

        category.Should().NotBeNull();
        category.Name.Should().Be(validCategory.Name);
        category.Description.Should().Be(validCategory.Description);
        category.Id.Should().NotBeEmpty();
        category.CreatedAt.Should().NotBeSameDateAs(default(DateTime));
        
        (category.CreatedAt > datetimeBefore).Should().BeTrue();
        (category.CreatedAt < datetimeAfter).Should().BeTrue();
        category.IsActive.Should().BeTrue();
    }

    [Theory(DisplayName = "InstantiateWithIsActive")]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData(true)]
    [InlineData(false)]
    public void InstantiateWithIsActive(bool isActive)
    {
        var validCategory = _categoryTestFixture.GetValidCategory();

        var datetimeBefore = DateTime.Now;

        var category = new DomainEntity.Category(validCategory.Name, validCategory.Description, isActive);

        var datetimeAfter = DateTime.Now.AddMicroseconds(1);

        category.Should().NotBeNull();
        category.Name.Should().Be(validCategory.Name);
        category.Description.Should().Be(validCategory.Description);
        category.Id.Should().NotBeEmpty();
        category.CreatedAt.Should().NotBeSameDateAs(default(DateTime));
        (category.CreatedAt > datetimeBefore).Should().BeTrue();
        (category.CreatedAt < datetimeAfter).Should().BeTrue();
        category.IsActive.Should().Be(isActive);
    }

    [Theory(DisplayName = nameof(InstantiateErrorWhenNameIsEmpty))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void InstantiateErrorWhenNameIsEmpty(string name)
    {
        var validCategory = _categoryTestFixture.GetValidCategory();
        Action action = () => new DomainEntity.Category(name, validCategory.Description);
        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should not be empty or null");
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenDescriptionIsNull))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateErrorWhenDescriptionIsNull()
    {
        var validCategory = _categoryTestFixture.GetValidCategory();
        Action action = () => new DomainEntity.Category(validCategory.Name, null);
        var exception = Assert.Throws<EntityValidationException>(action);
        Assert.Equal("Description should not be null", exception.Message);
    }

    //Nome = mínimo 3 caracteres
    [Theory(DisplayName = nameof(InstantiateErrorWhenNameIsLess3Characters))]
    [Trait("Domain", "Category - Aggregates")]
    [MemberData(nameof(GetNameIsLess3Characters), parameters: 10)]
    public void InstantiateErrorWhenNameIsLess3Characters(string invalidName) 
    {
        var validCategory = _categoryTestFixture.GetValidCategory();
        Action action = () => new DomainEntity.Category(invalidName, validCategory.Description);
        action.Should().Throw<EntityValidationException>()
            .WithMessage ("Name should be at leats 3 caracters long");
    }

    public static IEnumerable<object[]> GetNameIsLess3Characters(int numberOfTests = 6)
    {
        var fixture = new CategoryTestFixture();
        for (int i = 0; i< numberOfTests; i++)
        {
            var isOdd = i % 2 == 1;
            yield return new object[] { fixture.GetValidCategoryName()[..(isOdd? 1: 2)] };
        }
    }

    //Nome deve ter no máximo 255 caracteres
    [Fact(DisplayName = nameof(InstantiateErrorWhenNameIsGreaterThan255Chracters))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateErrorWhenNameIsGreaterThan255Chracters()
    {
        var validCategory = _categoryTestFixture.GetValidCategory();
        var invalidName = String.Join(null, Enumerable.Range(0, 256).Select(_ => "a").ToArray());
        Action action = () => new DomainEntity.Category(invalidName, validCategory.Description);
        action.Should().Throw<EntityValidationException>()
            .WithMessage("Name should be less or equal 255 characters");
    }
    //descricao deve ter no maximo 10_000 caracteres
    [Fact(DisplayName = nameof(InstantiateErrorWhenDescriptionIsGreaterThan10_000Chracters))]
    [Trait("Domain", "Category - Aggregates")]

    public void InstantiateErrorWhenDescriptionIsGreaterThan10_000Chracters()
    {
        var validCategory = _categoryTestFixture.GetValidCategory();
        var invalidDescription = String.Join(null, Enumerable.Range(0, 10001).Select(_ => "a").ToArray());
        Action action = () => new DomainEntity.Category(validCategory.Name, invalidDescription);
        action.Should().Throw<EntityValidationException>()
            .WithMessage("Description should be less or equal 10000 characters");
    }

    [Fact(DisplayName = "Activate")]
    [Trait("Domain", "Category - Aggregates")]
    
    public void Activate()
    {
        var validCategory = _categoryTestFixture.GetValidCategory();

        var category = new DomainEntity.Category(validCategory.Name, validCategory.Description, false);
        category.Activate();
        
        category.IsActive.Should().BeTrue();
    }

    [Fact(DisplayName = "Deactivate")]
    [Trait("Domain", "Category - Aggregates")]

    public void Deactivate()
    {
        var validCategory = _categoryTestFixture.GetValidCategory();

        var category = new DomainEntity.Category(validCategory.Name, validCategory.Description, false);
        category.Deactivate();

        category.IsActive.Should().BeFalse();
    }

    [Fact(DisplayName = "Update")]
    [Trait("Domain", "Category - Aggregates")]
    public void Update()
    {
        var validCategory = _categoryTestFixture.GetValidCategory();
        var category = new DomainEntity.Category(validCategory.Name, validCategory.Description);
        var newValues = _categoryTestFixture.GetValidCategory();

        category.Update(newValues.Name, newValues.Description);

        category.Name.Should().Be(newValues.Name);
        category.Description.Should().Be(newValues.Description);
    }

    [Fact(DisplayName = nameof(UpdateOnlyName))]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateOnlyName()
    {
        var category = _categoryTestFixture.GetValidCategory();
        var newValues = new { Name = _categoryTestFixture.GetValidCategoryName(), };
        var currentDescription = category.Description;
        category.Update(newValues.Name);

        category.Name.Should().Be(newValues.Name);
        category.Description.Should().Be(currentDescription);
    }

    [Theory(DisplayName = nameof(UpdateErrorWhenNameIsEmpty))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void UpdateErrorWhenNameIsEmpty(string? name)
    {
        var category = _categoryTestFixture.GetValidCategory();
        Action action = () => category.Update(name!);
        action.Should().Throw<EntityValidationException>()
            .WithMessage("Name should not be empty or null");
    }

    [Theory(DisplayName = nameof(UpdateErrorWhenNameIsLess3Characters))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("1")]
    [InlineData("12")]
    [InlineData("ca")]
    [InlineData("a")]
    public void UpdateErrorWhenNameIsLess3Characters(string invalidName)
    {
        var category = _categoryTestFixture.GetValidCategory();
        Action action = () => category.Update(invalidName);
        action.Should().Throw<EntityValidationException>()
            .WithMessage("Name should be at leats 3 caracters long");
    }

    [Fact(DisplayName = nameof(UpdateErrorWhenNameIsGreaterThan255Chracters))]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateErrorWhenNameIsGreaterThan255Chracters()
    {
        var invalidName = _categoryTestFixture.Faker.Lorem.Letter(256);
        var category = _categoryTestFixture.GetValidCategory();
        Action action = () => category.Update(invalidName);
        action.Should().Throw<EntityValidationException>()
            .WithMessage("Name should be less or equal 255 characters");
    }

    [Fact(DisplayName = nameof(UpdateErrorWhenDescriptionIsGreaterThan10_000Chracters))]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateErrorWhenDescriptionIsGreaterThan10_000Chracters()
    {
        var invalidDescription = _categoryTestFixture.Faker.Lorem.Letter(10_001);
        var category = _categoryTestFixture.GetValidCategory();
        Action action = () => category.Update(_categoryTestFixture.GetValidCategoryName(), invalidDescription);
        action.Should().Throw<EntityValidationException>()
            .WithMessage("Description should be less or equal 10000 characters");
    }
}

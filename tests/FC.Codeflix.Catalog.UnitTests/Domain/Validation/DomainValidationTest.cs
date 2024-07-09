using Bogus;

using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Domain.Validation;

using FluentAssertions;
namespace FC.Codeflix.Catalog.UnitTests.Domain.Validation;
public class DomainValidationTest
{
    private Faker Faker { get; set; } = new Faker();
    /*
     * if(String.IsNullOrWhiteSpace(Name))
            throw new EntityValidationException($"{nameof(Name)} should not be empty or null");
        if (Description == null)
            throw new EntityValidationException($"{nameof(Description)} should not be null");
        if (Name.Length < 3)
            throw new EntityValidationException("Name should be at leats 3 caracters long");
        if (Name.Length > 255)
            throw new EntityValidationException("Name should be less or equal 255 characters");
        if (Description.Length > 10000)
            throw new EntityValidationException("Description should be less or equal 10.000 characters");
     */

    [Fact(DisplayName = nameof(NotNullOk))]
    [Trait("Domain", $"DomainValidation - Validation")]
    public void NotNullOk()
    {
        var value = Faker.Commerce.ProductName();
        var fieldName = Faker.Commerce.Categories(1)[0];
        Action action = () => DomainValidation.NotNull(value, fieldName);
        action.Should().NotThrow();
    }

    [Fact(DisplayName = nameof(NotNullThrowWhenNull))]
    [Trait("Domain", $"DomainValidation - Validation")]
    public void NotNullThrowWhenNull()
    {
        string value = null;
        var fieldName = Faker.Commerce.Categories(1)[0];
        Action action = () => DomainValidation.NotNull(value, fieldName);
        action.Should().Throw<EntityValidationException>()
            .WithMessage($"{fieldName} should not be null");
    }
    
    [Theory(DisplayName = nameof(NotNullOrEmptyThrowWhenEmpty))]
    [Trait("Domain", $"DomainValidation - Validation")]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void NotNullOrEmptyThrowWhenEmpty(string? target)
    {
        var fieldName = Faker.Commerce.Categories(1)[0];
        Action action = () => DomainValidation.NotNullOrEmpty(target, fieldName );
        action.Should().Throw<EntityValidationException>()
            .WithMessage($"{fieldName} should not be empty or null");
    }

    [Fact(DisplayName = nameof(NotNullOrEmptyOk))]
    [Trait("Domain", $"DomainValidation - Validation")]
    public void NotNullOrEmptyOk()
    {
        var target = Faker.Commerce.ProductName();
        var fieldName = Faker.Commerce.Categories(1)[0];
        Action action = () => DomainValidation.NotNullOrEmpty(target, fieldName);
        action.Should().NotThrow();
    }

    [Theory(DisplayName = nameof(MinLengthThrowWhenLess))]
    [Trait("Domain", $"DomainValidation - Validation")]
    [MemberData(nameof(GetValuesSmallerThanMin), parameters:10)]
    public void MinLengthThrowWhenLess(string? target, int minLength)
    {
        var fieldName = Faker.Commerce.Categories(1)[0];
        Action action = () => DomainValidation.MinLength(target, minLength, fieldName);
        action.Should().Throw<EntityValidationException>()
            .WithMessage($"{fieldName} should be at leats {minLength} caracters long");
    }
    
    [Theory(DisplayName = nameof(MinLengthThrowWhenLess))]
    [Trait("Domain", $"DomainValidation - Validation")]
    [MemberData(nameof(GetValuesGreaterThanMin), parameters:10)]
    public void MinLengthOk(string? target, int minLength)
    {
        var fieldName = Faker.Commerce.Categories(1)[0];
        Action action = () => DomainValidation.MinLength(target, minLength, fieldName);
        action.Should().NotThrow();
    }

    public static IEnumerable<object[]> GetValuesSmallerThanMin(int numberOfTests = 5)
    {
        yield return new object[] { "123456", 10};
        var faker = new Faker();
        for (int i = 0; i < numberOfTests - 1; i++)
        {
            var example = faker.Commerce.ProductName();
            var minLength = example.Length + (new Random()).Next(1, 20);
            yield return new object[] { example, minLength };
        }
    }
    public static IEnumerable<object[]> GetValuesGreaterThanMin(int numberOfTests = 5)
    {
        yield return new object[] { "123456", 6};
        var faker = new Faker();
        for (int i = 0; i < numberOfTests - 1; i++)
        {
            var example = faker.Commerce.ProductName();
            var minLength = example.Length - (new Random()).Next(1, 5);
            yield return new object[] { example, minLength };
        }
    }

    [Theory(DisplayName = nameof(MaxLengthThrowWhenGreater))]
    [Trait("Domain", $"DomainValidation - Validation")]
    [MemberData(nameof(GetValuesGreaterThanMax), parameters: 10)]
    public void MaxLengthThrowWhenGreater(string? target, int maxLength)
    {
        var fieldName = Faker.Commerce.Categories(1)[0];
        Action action = () => DomainValidation.MaxLength(target, maxLength, fieldName);
        action.Should().Throw<EntityValidationException>()
            .WithMessage($"{fieldName} should be less or equal {maxLength} characters");
    }

    [Theory(DisplayName = nameof(MaxLengthOk))]
    [Trait("Domain", $"DomainValidation - Validation")]
    [MemberData(nameof(GetValuesLessThanMax), parameters: 10)]
    public void MaxLengthOk(string? target, int maxLength)
    {
        var fieldName = Faker.Commerce.Categories(1)[0];
        Action action = () => DomainValidation.MaxLength(target, maxLength, fieldName);
        action.Should().NotThrow();
    }

    public static IEnumerable<object[]> GetValuesGreaterThanMax(int numberOfTests = 5)
    {
        yield return new object[] { "123456", 5 };
        var faker = new Faker();
        for (int i = 0; i < numberOfTests - 1; i++)
        {
            var example = faker.Commerce.ProductName();
            var maxLength = example.Length - (new Random()).Next(1, 5);
            yield return new object[] { example, maxLength };
        }
    }
    public static IEnumerable<object[]> GetValuesLessThanMax(int numberOfTests = 5)
    {
        yield return new object[] { "123456", 6 };
        var faker = new Faker();
        for (int i = 0; i < numberOfTests - 1; i++)
        {
            var example = faker.Commerce.ProductName();
            var minLength = example.Length + (new Random()).Next(1, 10);
            yield return new object[] { example, minLength };
        }
    }
}

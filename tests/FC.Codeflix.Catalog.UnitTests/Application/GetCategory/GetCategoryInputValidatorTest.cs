using FC.Codeflix.Catalog.Application.UseCases.Category.GetCategory;
using FC.Codeflix.Catalog.UnitTests.Application.CreateCategory;

using FluentAssertions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FC.Codeflix.Catalog.UnitTests.Application.GetCategory;
[Collection(nameof(CreateCategoryTestFixture))]
public class GetCategoryInputValidatorTest
{
    private readonly CreateCategoryTestFixture _fixture;

    public GetCategoryInputValidatorTest(CreateCategoryTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(ValidationOk))]
    [Trait("Application", "GetCategoryInputValidation - UseCases")]
    public void ValidationOk()
    {
        var inputValid = new GetCategoryInput(Guid.NewGuid());
        var validator = new GetCategoryInputValidator();

        var validateResult = validator.Validate(inputValid);

        validateResult.Should().NotBeNull();
        validateResult.IsValid.Should().BeTrue();
        validateResult.Errors.Should().HaveCount(0);
    }

    [Fact(DisplayName = nameof(InvalidWhenEmptyGuidId))]
    [Trait("Application", "GetCategoryInputValidation - UseCases")]
    public void InvalidWhenEmptyGuidId()
    {
        var invalidInput = new GetCategoryInput(Guid.Empty);
        var validator = new GetCategoryInputValidator();

        var invalidResult = validator.Validate(invalidInput);

        invalidResult.Should().NotBeNull();
        invalidResult.IsValid.Should().BeFalse();
        invalidResult.Errors.Should().HaveCount(1);
        invalidResult.Errors[0].ErrorMessage.Should().Contain("'Id'");
    }
}

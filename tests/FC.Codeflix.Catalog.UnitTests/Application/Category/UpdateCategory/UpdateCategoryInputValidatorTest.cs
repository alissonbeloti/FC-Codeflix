﻿using FC.Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;
using FluentAssertions;

namespace FC.Codeflix.Catalog.UnitTests.Application.UpdateCategory;
[Collection(nameof(UpdateCategoryTestFixture))]
public class UpdateCategoryInputValidatorTest
{
    private readonly UpdateCategoryTestFixture _fixture;

    public UpdateCategoryInputValidatorTest(UpdateCategoryTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = "")]
    [Trait("Application", "UpdateCategoryInputValidatorTest - Use Cases")]
    public void DontValidateWhenEmptyGuid()
    {
        var input = _fixture.GetValidInput(Guid.Empty);
        var validator = new UpdateCategoryInputValidator();

        var validateResult = validator.Validate(input);

        validateResult.Should().NotBeNull();
        validateResult.IsValid.Should().BeFalse();
        validateResult.Errors.Should().HaveCount(1);
        validateResult.Errors[0].ErrorMessage.Should().Contain("'Id'");
    }

    [Fact(DisplayName = "")]
    [Trait("Application", "UpdateCategoryInputValidatorTest - Use Cases")]
    public void DontValidateWhenValid()
    {
        var input = _fixture.GetValidInput();
        var validator = new UpdateCategoryInputValidator();

        var validateResult = validator.Validate(input);

        validateResult.Should().NotBeNull();
        validateResult.IsValid.Should().BeTrue();
        validateResult.Errors.Should().HaveCount(0);

    }
}

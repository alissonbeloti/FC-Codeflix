using FC.Codeflix.Catalog.Domain.Validation;
using FC.Codeflix.Catalog.Domain.Validator;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

using FluentAssertions;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.Video;
[Collection(nameof(VideoTestFixture))]
public class VideoValidatorTest(VideoTestFixture fixture)
{
    [Fact(DisplayName = nameof(ReturnsValidWhenVideoIsValid))]
    [Trait("Domain", "VideoValidator - Validators")]
    public void ReturnsValidWhenVideoIsValid()
    {
        var validVideo = fixture.GetValidVideo();
        var notificationValidationHandler = new NotificationValidationHandler();
        var videoValidator = new VideoValidator(validVideo, notificationValidationHandler);

        videoValidator.Validate();

        notificationValidationHandler.HasErrors().Should().BeFalse();
        notificationValidationHandler.Errors.Should().HaveCount(0);
    }

    [Fact(DisplayName = nameof(ReturnsErrorWhenTitleIsTooLong))]
    [Trait("Domain", "VideoValidator - Validators")]
    public void ReturnsErrorWhenTitleIsTooLong()
    {
        var invalidVideo = new DomainEntity.Video(
            fixture.GetTooLongTitle(),
            fixture.GetValidDescription(),
            fixture.GetRandoBoolean(),
            fixture.GetRandoBoolean(),
            fixture.GetValidYearLaunched(),
            fixture.GetValidDuration(),
            fixture.GetRandomRationg()
            );
        var notificationValidationHandler = new NotificationValidationHandler();
        var videoValidator = new VideoValidator(invalidVideo, notificationValidationHandler);

        videoValidator.Validate();

        notificationValidationHandler.HasErrors().Should().BeTrue();
        notificationValidationHandler.Errors.Should().HaveCount(1);
        notificationValidationHandler.Errors.FirstOrDefault()!.Message.Should()
            .Be("'Title' should be less or equal 255 characters long");
    }

    [Fact(DisplayName = nameof(ReturnsErrorsWhenTitleIsEmpty))]
    [Trait("Domain", "VideoValidator - Validators")]
    public void ReturnsErrorsWhenTitleIsEmpty()
    {
        var invalidVideo = new DomainEntity.Video(
            "",
            fixture.GetValidDescription(),
            fixture.GetRandoBoolean(),
            fixture.GetRandoBoolean(),
            fixture.GetValidYearLaunched(),
            fixture.GetValidDuration(),
            fixture.GetRandomRationg()
            );
        var notificationValidationHandler = new NotificationValidationHandler();
        var videoValidator = new VideoValidator(invalidVideo, notificationValidationHandler);

        videoValidator.Validate();

        notificationValidationHandler.HasErrors().Should().BeTrue();
        notificationValidationHandler.Errors.Should().HaveCount(1);
        notificationValidationHandler.Errors.FirstOrDefault()!.Message.Should()
            .Be("'Title' is required");
    }

    [Fact(DisplayName = nameof(ReturnsErrorsWhenDescriptionIsEmpty))]
    [Trait("Domain", "VideoValidator - Validators")]
    public void ReturnsErrorsWhenDescriptionIsEmpty()
    {
        var invalidVideo = new DomainEntity.Video(
            fixture.GetValidTitle(),
            "",
            fixture.GetRandoBoolean(),
            fixture.GetRandoBoolean(),
            fixture.GetValidYearLaunched(),
            fixture.GetValidDuration(),
            fixture.GetRandomRationg()
            );
        var notificationValidationHandler = new NotificationValidationHandler();
        var videoValidator = new VideoValidator(invalidVideo, notificationValidationHandler);

        videoValidator.Validate();

        notificationValidationHandler.HasErrors().Should().BeTrue();
        notificationValidationHandler.Errors.Should().HaveCount(1);
        notificationValidationHandler.Errors.FirstOrDefault()!.Message.Should()
            .Be("'Description' is required");
    }

    [Fact(DisplayName = nameof(ReturnsErrorsWhenDescriptionIsTooLong))]
    [Trait("Domain", "VideoValidator - Validators")]
    public void ReturnsErrorsWhenDescriptionIsTooLong()
    {
        var invalidVideo = new DomainEntity.Video(
            fixture.GetValidTitle(),
            fixture.GetTooLongDescription(),
            fixture.GetRandoBoolean(),
            fixture.GetRandoBoolean(),
            fixture.GetValidYearLaunched(),
            fixture.GetValidDuration(),
            fixture.GetRandomRationg()
            );
        var notificationValidationHandler = new NotificationValidationHandler();
        var videoValidator = new VideoValidator(invalidVideo, notificationValidationHandler);

        videoValidator.Validate();

        notificationValidationHandler.HasErrors().Should().BeTrue();
        notificationValidationHandler.Errors.Should().HaveCount(1);
        notificationValidationHandler.Errors.FirstOrDefault()!.Message.Should()
            .Be("'Description' should be less or equal 4000 characters long");
    }
}

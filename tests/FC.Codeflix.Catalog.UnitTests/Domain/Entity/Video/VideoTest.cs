using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Domain.Validation;

using FluentAssertions;

using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.Video;

[Collection(nameof(VideoTestFixture))]
public class VideoTest(VideoTestFixture fixture)
{
    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "Video - Aggregates")]
    public void Instantiate()
    {
        var title = fixture.GetValidTitle();
        var description = fixture.GetValidDescription();
        var opened = fixture.GetRandoBoolean();
        var published = fixture.GetRandoBoolean();
        var yearLaunched = fixture.GetValidYearLaunched();
        var duration = fixture.GetValidDuration();
        var expectedRating = Rating.ER;

        var video = new DomainEntity.Video(
            title,
            description,
            opened,
            published,
            yearLaunched,
            duration,
            expectedRating
        );


        video.Title.Should().Be(title);
        video.Description.Should().Be(description);
        video.Opened.Should().Be(opened);
        video.Published.Should().Be(published);
        video.YearLaunched.Should().Be(yearLaunched);
        video.Duration.Should().Be(duration);
        video.Rating.Should().Be(expectedRating);
        video.Thumb.Should().BeNull();
        video.ThumbHalf.Should().BeNull();
        video.Banner.Should().BeNull();
        video.Media.Should().BeNull();
        video.Trailer.Should().BeNull();
    }

    [Fact(DisplayName = nameof(Update))]
    [Trait("Domain", "Video - Aggregates")]
    public void Update()
    {
        var title = fixture.GetValidTitle();
        var description = fixture.GetValidDescription();
        var opened = fixture.GetRandoBoolean();
        var published = fixture.GetRandoBoolean();
        var yearLaunched = fixture.GetValidYearLaunched();
        var duration = fixture.GetValidDuration();
        var video = fixture.GetValidVideo();

        video.Update(
            title, description, opened, published, yearLaunched, duration);

        video.Title.Should().Be(title);
        video.Description.Should().Be(description);
        video.Opened.Should().Be(opened);
        video.Published.Should().Be(published);
        video.YearLaunched.Should().Be(yearLaunched);
        video.Duration.Should().Be(duration);

    }

    [Fact(DisplayName = nameof(UpdateWithRating))]
    [Trait("Domain", "Video - Aggregates")]
    public void UpdateWithRating()
    {
        var title = fixture.GetValidTitle();
        var description = fixture.GetValidDescription();
        var opened = fixture.GetRandoBoolean();
        var published = fixture.GetRandoBoolean();
        var yearLaunched = fixture.GetValidYearLaunched();
        var duration = fixture.GetValidDuration();
        var expectedRating = fixture.GetRandomRationg();
        var video = fixture.GetValidVideo();

        video.Update(
            title, description, opened, published, yearLaunched, duration,
            expectedRating);

        video.Title.Should().Be(title);
        video.Description.Should().Be(description);
        video.Opened.Should().Be(opened);
        video.Published.Should().Be(published);
        video.YearLaunched.Should().Be(yearLaunched);
        video.Duration.Should().Be(duration);
        video.Rating.Should().Be(expectedRating);
    }

    [Fact(DisplayName = nameof(UpdateWithoutRatingDoesntTheRating))]
    [Trait("Domain", "Video - Aggregates")]
    public void UpdateWithoutRatingDoesntTheRating()
    {
        var title = fixture.GetValidTitle();
        var description = fixture.GetValidDescription();
        var opened = fixture.GetRandoBoolean();
        var published = fixture.GetRandoBoolean();
        var yearLaunched = fixture.GetValidYearLaunched();
        var duration = fixture.GetValidDuration();
        var video = fixture.GetValidVideo();
        var expectedRating = video.Rating;

        video.Update(
            title, description, opened, published, yearLaunched, duration);

        video.Rating.Should().Be(expectedRating);
    }

    [Fact(DisplayName = nameof(ValidateStillValidatingAfterUpdateToValidState))]
    [Trait("Domain", "Video - Aggregates")]
    public void ValidateStillValidatingAfterUpdateToValidState()
    {
        var title = fixture.GetValidTitle();
        var description = fixture.GetValidDescription();
        var opened = fixture.GetRandoBoolean();
        var published = fixture.GetRandoBoolean();
        var yearLaunched = fixture.GetValidYearLaunched();
        var duration = fixture.GetValidDuration();
        var video = fixture.GetValidVideo();
        video.Update(
            title, description, opened, published, yearLaunched, duration);
        var notificationHandle = new NotificationValidationHandler();

        video.Validate(notificationHandle);

        notificationHandle.Should().NotBeNull();
        notificationHandle.HasErrors().Should().BeFalse();
    }

    [Fact(DisplayName = nameof(ValidateStillValidatingAfterUpdateToInvalidState))]
    [Trait("Domain", "Video - Aggregates")]
    public void ValidateStillValidatingAfterUpdateToInvalidState()
    {
        var title = "  ";
        var description = fixture.GetValidDescription();
        var opened = fixture.GetRandoBoolean();
        var published = fixture.GetRandoBoolean();
        var yearLaunched = fixture.GetValidYearLaunched();
        var duration = fixture.GetValidDuration();
        var video = fixture.GetValidVideo();
        video.Update(
            title, description, opened, published, yearLaunched, duration);
        var notificationHandle = new NotificationValidationHandler();

        video.Validate(notificationHandle);

        notificationHandle.Should().NotBeNull();
        notificationHandle.HasErrors().Should().BeTrue();
        notificationHandle.Errors.Should().HaveCount(1);
    }

    [Fact(DisplayName = nameof(ValidateWhenValidState))]
    [Trait("Domain", "Video - Aggregates")]
    public void ValidateWhenValidState()
    {
        var validVideo = fixture.GetValidVideo();
        var notificationHandler = new NotificationValidationHandler();

        validVideo.Validate(notificationHandler);

        notificationHandler.HasErrors().Should().BeFalse();
    }

    [Fact(DisplayName = nameof(ValidateWithErrorWhenInvalidState))]
    [Trait("Domain", "Video - Aggregates")]
    public void ValidateWithErrorWhenInvalidState()
    {
        var invalidVideo = new DomainEntity.Video(
            fixture.GetTooLongTitle(),
            fixture.GetTooLongDescription(),
            fixture.GetRandoBoolean(),
            fixture.GetRandoBoolean(),
            fixture.GetValidYearLaunched(),
            fixture.GetValidDuration(),
            fixture.GetRandomRationg()
            );
        var notificationHandler = new NotificationValidationHandler();

        invalidVideo.Validate(notificationHandler);

        notificationHandler.HasErrors().Should().BeTrue();
        notificationHandler.Errors.Should().BeEquivalentTo(new List<ValidationError>
            {
            new ValidationError("'Title' should be less or equal 255 characters long"),
            new ValidationError("'Description' should be less or equal 4000 characters long")
            });
    }

    [Fact(DisplayName = nameof(UpdateThumb))]
    [Trait("Domain", "Video - Aggregates")]
    public void UpdateThumb()
    {
        var validVideo = fixture.GetValidVideo();
        var validImagePath = fixture.GetValidImagePath();

        validVideo.UpdateThumb(validImagePath);

        validVideo.Thumb.Should().NotBeNull();
        validVideo.Thumb!.Path.Should().Be(validImagePath);
    }

    [Fact(DisplayName = nameof(UpdateThumbHalf))]
    [Trait("Domain", "Video - Aggregates")]
    public void UpdateThumbHalf()
    {
        var validVideo = fixture.GetValidVideo();
        var validImagePath = fixture.GetValidImagePath();

        validVideo.UpdateThumbHalf(validImagePath);

        validVideo.ThumbHalf.Should().NotBeNull();
        validVideo.ThumbHalf!.Path.Should().Be(validImagePath);
    }

    [Fact(DisplayName = nameof(UpdateBanner))]
    [Trait("Domain", "Video - Aggregates")]
    public void UpdateBanner()
    {
        var validVideo = fixture.GetValidVideo();
        var validImagePath = fixture.GetValidImagePath();

        validVideo.UpdateBanner(validImagePath);

        validVideo.Banner.Should().NotBeNull();
        validVideo.Banner!.Path.Should().Be(validImagePath);
    }

    [Fact(DisplayName = nameof(UpdateMedia))]
    [Trait("Domain", "Video - Aggregates")]
    public void UpdateMedia()
    {
        var validMedia = fixture.GetValidVideo();
        string validPath = fixture.GetValidMediaPath();

        validMedia.UpdateMedia(validPath);

        validMedia.Media.Should().NotBeNull();
        validMedia.Media!.FilePath.Should().Be(validPath);
        validMedia.Events.Should().HaveCount(1);
    }

    [Fact(DisplayName = nameof(UpdateTrailer))]
    [Trait("Domain", "Video - Aggregates")]
    public void UpdateTrailer()
    {
        var validMedia = fixture.GetValidVideo();
        string validPath = fixture.GetValidMediaPath();

        validMedia.UpdateTrailer(validPath);

        validMedia.Trailer.Should().NotBeNull();
        validMedia.Trailer!.FilePath.Should().Be(validPath);
    }

    [Fact(DisplayName = nameof(UpdateAsSentToEncode))]
    [Trait("Domain", "Video - Aggregates")]
    public void UpdateAsSentToEncode()
    {
        var validVideo = fixture.GetValidVideo();
        string validPath = fixture.GetValidMediaPath();
        validVideo.UpdateMedia(validPath);

        validVideo.UpdateAsSentToEncode();

        validVideo.Media.Should().NotBeNull();
        validVideo.Media!.Status.Should().Be(MediaStatus.Processing);
    }

    [Fact(DisplayName = nameof(UpdateAsSentToEncodeThrowsWhenThereIsNoMedia))]
    [Trait("Domain", "Video - Aggregates")]
    public void UpdateAsSentToEncodeThrowsWhenThereIsNoMedia()
    {
        var validVideo = fixture.GetValidVideo();

        var action = () => validVideo.UpdateAsSentToEncode();

        action.Should().NotBeNull();
        action.Should().Throw<EntityValidationException>()
            .WithMessage("There is no Media");
    }

    [Fact(DisplayName = nameof(UpdateAsEncoded))]
    [Trait("Domain", "Video - Aggregates")]
    public void UpdateAsEncoded()
    {
        var validVideo = fixture.GetValidVideo();
        string validPath = fixture.GetValidMediaPath();
        string validEncodedPath = fixture.GetValidMediaPath();
        validVideo.UpdateMedia(validPath);

        validVideo.UpdateAsEncoded(validEncodedPath);

        validVideo.Media.Should().NotBeNull();
        validVideo.Media!.EncondedPath.Should().Be(validEncodedPath);
    }

    [Fact(DisplayName = nameof(UpdateAsEncodedError))]
    [Trait("Domain", "Video - Aggregates")]
    public void UpdateAsEncodedError()
    {
        var validVideo = fixture.GetValidVideo();
        string validPath = fixture.GetValidMediaPath();
        string validEncodedPath = fixture.GetValidMediaPath();
        validVideo.UpdateMedia(validPath);

        validVideo.UpdateAsEncodingError();

        validVideo.Media.Should().NotBeNull();
        validVideo.Media!.Status.Should().Be(MediaStatus.Error);
        validVideo.Media.EncondedPath.Should().BeNull();
    }


    [Fact(DisplayName = nameof(UpdateAsEncodedThrowsWhenThereIsNoMedia))]
    [Trait("Domain", "Video - Aggregates")]
    public void UpdateAsEncodedThrowsWhenThereIsNoMedia()
    {
        var validVideo = fixture.GetValidVideo();

        var action = () => validVideo.UpdateAsEncoded(fixture.GetValidMediaPath());

        action.Should().NotBeNull();
        action.Should().Throw<EntityValidationException>()
            .WithMessage("There is no Media");
    }

    [Fact(DisplayName = nameof(UpdateAsEncodingErrorThrowsWhenThereIsNoMedia))]
    [Trait("Domain", "Video - Aggregates")]
    public void UpdateAsEncodingErrorThrowsWhenThereIsNoMedia()
    {
        var validVideo = fixture.GetValidVideo();

        var action = () => validVideo.UpdateAsEncodingError();

        action.Should().NotBeNull();
        action.Should().Throw<EntityValidationException>()
            .WithMessage("There is no Media");
    }

    [Fact(DisplayName = nameof(AddCategory))]
    [Trait("Domain", "Video - Aggregates")]
    public void AddCategory()
    {
        var validVideo = fixture.GetValidVideo();
        var categoryIdExample = Guid.NewGuid();

        validVideo.AddCategory(categoryIdExample);

        validVideo.Categories.Should().HaveCount(1);
        validVideo.Categories[0]!.Should().Be(categoryIdExample);
    }

    [Fact(DisplayName = nameof(RemoveCategory))]
    [Trait("Domain", "Video - Aggregates")]
    public void RemoveCategory()
    {
        var validVideo = fixture.GetValidVideo();
        var categoryIdExample = Guid.NewGuid();
        var categoryIdExample2 = Guid.NewGuid();
        validVideo.AddCategory(categoryIdExample);
        validVideo.AddCategory(categoryIdExample2);

        validVideo.RemoveCategory(categoryIdExample);

        validVideo.Categories.Should().HaveCount(1);
        validVideo.Categories[0]!.Should().Be(categoryIdExample2);
    }

    [Fact(DisplayName = nameof(RemoveAllCategories))]
    [Trait("Domain", "Video - Aggregates")]
    public void RemoveAllCategories()
    {
        var validVideo = fixture.GetValidVideo();
        var categoryIdExample = Guid.NewGuid();
        var categoryIdExample2 = Guid.NewGuid();
        validVideo.AddCategory(categoryIdExample);
        validVideo.AddCategory(categoryIdExample2);

        validVideo.RemoveAllCategories();

        validVideo.Categories.Should().HaveCount(0);
        
    }

    [Fact(DisplayName = nameof(AddGenre))]
    [Trait("Domain", "Video - Aggregates")]
    public void AddGenre()
    {
        var validVideo = fixture.GetValidVideo();
        var exampleId = Guid.NewGuid();

        validVideo.AddGenre(exampleId);

        validVideo.Genres.Should().HaveCount(1);
        validVideo.Genres[0]!.Should().Be(exampleId);
    }

    [Fact(DisplayName = nameof(RemoveGenre))]
    [Trait("Domain", "Video - Aggregates")]
    public void RemoveGenre()
    {
        var validVideo = fixture.GetValidVideo();
        var genreIdExample = Guid.NewGuid();
        var genreIdExample2 = Guid.NewGuid();
        validVideo.AddGenre(genreIdExample);
        validVideo.AddGenre(genreIdExample2);

        validVideo.RemoveGenre(genreIdExample);

        validVideo.Genres.Should().HaveCount(1);
        validVideo.Genres[0]!.Should().Be(genreIdExample2);
    }

    [Fact(DisplayName = nameof(RemoveAllGenres))]
    [Trait("Domain", "Video - Aggregates")]
    public void RemoveAllGenres()
    {
        var validVideo = fixture.GetValidVideo();
        var genreIdExample = Guid.NewGuid();
        var genreIdExample2 = Guid.NewGuid();
        validVideo.AddGenre(genreIdExample);
        validVideo.AddGenre(genreIdExample2);

        validVideo.RemoveAllGenres();

        validVideo.Genres.Should().HaveCount(0);

    }

    [Fact(DisplayName = nameof(AddCastMember))]
    [Trait("Domain", "Video - Aggregates")]
    public void AddCastMember()
    {
        var validVideo = fixture.GetValidVideo();
        var exampleId = Guid.NewGuid();

        validVideo.AddCastMember(exampleId);

        validVideo.CastMembers.Should().HaveCount(1);
        validVideo.CastMembers[0]!.Should().Be(exampleId);
    }

    [Fact(DisplayName = nameof(RemoveCastMember))]
    [Trait("Domain", "Video - Aggregates")]
    public void RemoveCastMember()
    {
        var validVideo = fixture.GetValidVideo();
        var casMemberIdExample = Guid.NewGuid();
        var casMemberIdExample2 = Guid.NewGuid();
        validVideo.AddCastMember(casMemberIdExample);
        validVideo.AddCastMember(casMemberIdExample2);

        validVideo.RemoveCastMember(casMemberIdExample);

        validVideo.CastMembers.Should().HaveCount(1);
        validVideo.CastMembers[0]!.Should().Be(casMemberIdExample2);
    }

    [Fact(DisplayName = nameof(RemoveAllCastMembers))]
    [Trait("Domain", "Video - Aggregates")]
    public void RemoveAllCastMembers()
    {
        var validVideo = fixture.GetValidVideo();
        var casMemberIdExample = Guid.NewGuid();
        var casMemberIdExample2 = Guid.NewGuid();
        validVideo.AddCastMember(casMemberIdExample);
        validVideo.AddCastMember(casMemberIdExample2);

        validVideo.RemoveAllCastMembers();

        validVideo.CastMembers.Should().HaveCount(0);

    }
}

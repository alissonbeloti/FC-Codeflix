using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Domain.SeedWork;
using FC.Codeflix.Catalog.Domain.Validation;
using FC.Codeflix.Catalog.Domain.Validator;
using FC.Codeflix.Catalog.Domain.ValueObject;

namespace FC.Codeflix.Catalog.Domain.Entity;
public class Video: AggregateRoot
{
    

    public string Title { get; private set; }
    public string Description { get; private set; }
    public bool Opened { get; private set; }
    public bool Published { get; private set; }
    public int YearLaunched { get; private set; }
    public int Duration { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Rating Rating { get; private set; }
    public Image? Thumb { get; private set; }
    public Image? ThumbHalf { get; private set; }
    public Image? Banner { get; private set; }
    public Media? Media { get; private set; }
    public Media? Trailer { get; private set; }
    public IReadOnlyList<Guid> Categories => _categories.AsReadOnly();
    public IReadOnlyList<Guid> Genres => _genres.AsReadOnly();
    public IReadOnlyList<Guid> CastMembers => _castMembers.AsReadOnly();


    private List<Guid> _castMembers;
    private List<Guid> _categories;
    private List<Guid> _genres;

    public Video(string title, string description, bool opened, bool published, int yearLaunched, int duration, Rating rating)
    {
        Title = title;
        Description = description;
        Opened = opened;
        Published = published;
        YearLaunched = yearLaunched;
        Duration = duration;
        CreatedAt = DateTime.Now;
        Rating = rating;

        _categories = new();
        _genres = new();
        _castMembers = new();
    }

    public void Validate(ValidationHandler handler) 
        => new VideoValidator(this, handler).Validate();

    public void Update(string title, string description, bool opened, 
        bool published, int yearLaunched, int duration, Rating? rating = null)
    {
        Title = title;
        Description = description;
        Opened = opened;
        Published = published;
        YearLaunched = yearLaunched;
        Duration = duration;
        if (rating is not null) {
            Rating = rating.Value;
        }
    }

    public void UpdateThumb(string validImagePath) => 
        Thumb = new Image(validImagePath);

    public void UpdateThumbHalf(string validImagePath) => 
        ThumbHalf = new Image(validImagePath);

    public void UpdateBanner(string validImagePath) =>
        Banner = new Image(validImagePath);

    public void UpdateMedia(string validPath) =>
        Media = new Media(validPath);

    public void UpdateTrailer(string validPath) =>
        Trailer = new Media(validPath);

    public void UpdateAsSentToEncode()
    {
        if (Media is null)
            throw new EntityValidationException("There is no Media");
        Media!.UpdateAsSentToEncode();
    }

    public void UpdateAsEncoded(string validEncodedPath)
    {
        if (Media is null)
            throw new EntityValidationException("There is no Media");
        Media.UpdateAsEncoded(validEncodedPath);
    }

    public void AddCategory(Guid categoryId) => 
        _categories.Add(categoryId);

    public void RemoveCategory(Guid categoryId) =>
        _categories.Remove(categoryId);

    public void RemoveAllCategories() => 
        _categories = new();

    public void AddGenre(Guid genreId) =>
        _genres.Add(genreId);

    public void RemoveGenre(Guid genreId) =>
        _genres.Remove(genreId);
    public void RemoveAllGenres() =>
        _genres = new();

    public void AddCastMember(Guid castMemberId) =>
        _castMembers.Add(castMemberId);

    public void RemoveCastMember(Guid castMemberId) =>
        _castMembers.Remove(castMemberId);

    public void RemoveAllCastMembers() => _castMembers = new();

}

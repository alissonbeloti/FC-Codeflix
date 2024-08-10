
using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Domain.Validation;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.CreateVideo;
public class CreateVideo(IVideoRepository repository,
    ICategoryRepository categoryRepository,
    IGenreRepository genreRepository,
    ICastMemberRepository castMemberRepository,
    IUnitOfWork unitOfWork,
    IStorageService storageService) : ICreateVideo
{
    public async Task<CreateVideoOutput> Handle(CreateVideoInput request,
        CancellationToken cancellationToken)
    {
        var video = new Domain.Entity.Video(request.Title, request.Description,
            request.Opened,
            request.Published,
            request.YearLaunched,
            request.Duration,
            request.Rating);
        var valitationHandler = new NotificationValidationHandler();
        video.Validate(valitationHandler);
        if (valitationHandler.HasErrors())
            throw new EntityValidationException("There are validation errors",
                valitationHandler.Errors);

        await ValidateCategory(categoryRepository, request, video, cancellationToken);
        await ValidateGenres(genreRepository, request, video, cancellationToken);
        await ValidateCastMember(castMemberRepository, request, video, cancellationToken);
        try
        {
            await UploadImages(storageService, request, video, cancellationToken);

            await repository.Insert(video, cancellationToken);
            await unitOfWork.Commit(cancellationToken);
        }
        catch (Exception)
        {
            await ClearStorage(storageService, video, cancellationToken);
            throw;
        }

        return CreateVideoOutput.FromVideo(video);
    }

    private static async Task ClearStorage(IStorageService storageService, Domain.Entity.Video video, CancellationToken cancellationToken)
    {
        if (video.Thumb is not null)
            await storageService.Delete(video.Thumb.Path, cancellationToken);
        if (video.Banner is not null)
            await storageService.Delete(video.Banner.Path, cancellationToken);
        if (video.ThumbHalf is not null)
            await storageService.Delete(video.ThumbHalf.Path, cancellationToken);
    }

    private static async Task UploadImages(IStorageService storageService, CreateVideoInput request, Domain.Entity.Video video, CancellationToken cancellationToken)
    {
        if (request.Thumb is not null)
        {
            var thumbUrl = await storageService.Upload($"{video.Id}-thumb.{request.Thumb.Extension}",
                request.Thumb.FileStream, cancellationToken);
            video.UpdateThumb(thumbUrl);
        }
        if (request.Banner is not null)
        {
            var bannerUrl = await storageService.Upload($"{video.Id}-banner.{request.Banner!.Extension}",
                request.Banner.FileStream, cancellationToken);
            video.UpdateBanner(bannerUrl);
        }
        if (request.ThumbHalf is not null)
        {
            var thumbHalfUrl = await storageService.Upload($"{video.Id}-thumbhalf.{request.ThumbHalf!.Extension}",
                request.ThumbHalf.FileStream, cancellationToken);
            video.UpdateThumbHalf(thumbHalfUrl);
        }
    }

    private static async Task ValidateCastMember(ICastMemberRepository castMemberRepository, CreateVideoInput request, Domain.Entity.Video video, CancellationToken cancellationToken)
    {
        if ((request.CastMembersIds?.Count ?? 0) > 0)
        {
            var persistenceIds = await castMemberRepository
                .GetIdsListByIds(request.CastMembersIds!.ToList(), cancellationToken);
            if (persistenceIds.Count() < request.CastMembersIds!.Count)
            {
                var notFoundIds = request.CastMembersIds!.ToList().FindAll(x =>
                    !persistenceIds.Contains(x)
                );
                throw new RelatedAggregateException($"Related castMember id (or ids) not found: {string.Join(",", notFoundIds)}.");
            }
            request.CastMembersIds!.ToList().ForEach(video.AddCastMember);
        }
    }

    private static async Task ValidateGenres(IGenreRepository genreRepository, CreateVideoInput request, Domain.Entity.Video video, CancellationToken cancellationToken)
    {
        if ((request.GenresIds?.Count ?? 0) > 0)
        {
            var persistenceIds = await genreRepository
                .GetIdsListByIds(request.GenresIds!.ToList(), cancellationToken);
            if (persistenceIds.Count() < request.GenresIds!.Count)
            {
                var notFoundIds = request.GenresIds!.ToList().FindAll(x =>
                    !persistenceIds.Contains(x)
                );
                throw new RelatedAggregateException($"Related genre id (or ids) not found: {string.Join(",", notFoundIds)}.");
            }
            request.GenresIds!.ToList().ForEach(video.AddGenre);
        }
    }

    private static async Task ValidateCategory(ICategoryRepository categoryRepository, CreateVideoInput request, Domain.Entity.Video video, CancellationToken cancellationToken)
    {
        if ((request.CategoriesIds?.Count ?? 0) > 0)
        {
            var persistenceIds = await categoryRepository
                .GetIdsListByIds(request.CategoriesIds!.ToList(), cancellationToken);
            if (persistenceIds.Count() < request.CategoriesIds!.Count)
            {
                var notFoundIds = request.CategoriesIds!.ToList().FindAll(x =>
                    !persistenceIds.Contains(x)
                );
                throw new RelatedAggregateException($"Related category id (or ids) not found: {string.Join(",", notFoundIds)}.");
            }
            request.CategoriesIds!.ToList().ForEach(video.AddCategory);
        }
    }
}

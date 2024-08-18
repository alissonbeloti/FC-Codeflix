using FC.Codeflix.Catalog.Application.Common;
using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.Application.UseCases.Video.CreateVideo;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Domain.SeedWork;
using FC.Codeflix.Catalog.Domain.Validation;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.UpdateVideo;
public class UpdateVideo (IVideoRepository videoRepository,
    IGenreRepository genreRepository,
    ICategoryRepository categoryRepository,
    ICastMemberRepository castMemberRepository,
    IUnitOfWork unitOfWork,
    IStorageService storageService) : IUpdateVideo
{

    public async Task<VideoModelOutput> Handle(UpdateVideoInput request, CancellationToken cancellationToken)
    {
        var video = await videoRepository.Get(request.VideoId, cancellationToken);
        video.Update(request.Title, request.Description, request.Opened,
            request.Published, request.YearLaunched, request.Duration,
            request.Rating);

        await ValidateGenres(genreRepository, request, video, cancellationToken);
        await ValidateCategory(categoryRepository, request, video, cancellationToken);
        await ValidateCastMember(castMemberRepository, request, video, cancellationToken);

        var valitationHandler = new NotificationValidationHandler();
        video.Validate(valitationHandler);
        if (valitationHandler.HasErrors())
            throw new EntityValidationException("There are validation errors",
                valitationHandler.Errors);

        try
        {
            await UploadImages(storageService, request, video, cancellationToken);
            await videoRepository.Update(video, cancellationToken);
            await unitOfWork.Commit(cancellationToken);
        }
        catch (Exception)
        {
            await ClearStorage(storageService, video, request, cancellationToken);
            throw;
        }
        return VideoModelOutput.FromVideo(video);
    }
      

    private static async Task ClearStorage(IStorageService storageService, 
        Domain.Entity.Video video, UpdateVideoInput input,
        CancellationToken cancellationToken)
    {
        if (video.Thumb is not null && input.Thumb is not null)
            await storageService.Delete(video.Thumb.Path, cancellationToken);
        if (video.Banner is not null && input.Banner is not null)
            await storageService.Delete(video.Banner.Path, cancellationToken);
        if (video.ThumbHalf is not null && input.ThumbHalf is not null)
            await storageService.Delete(video.ThumbHalf.Path, cancellationToken);
       
    }

    private static async Task UploadImages(IStorageService storageService, UpdateVideoInput request, Domain.Entity.Video video, CancellationToken cancellationToken)
    {
        if (request.Thumb is not null)
        {
            var fileName = StorageName.Create(video.Id, nameof(video.Thumb), request.Thumb.Extension);
            var thumbUrl = await storageService.Upload(
                fileName,
                request.Thumb.FileStream,
                request.Thumb.ContentType,
                cancellationToken);
            video.UpdateThumb(thumbUrl);
        }
        if (request.Banner is not null)
        {
            var fileName = StorageName.Create(video.Id, nameof(video.Banner), request.Banner.Extension);
            var bannerUrl = await storageService.Upload(fileName,
                request.Banner.FileStream, 
                request.Banner.ContentType,
                cancellationToken);
            video.UpdateBanner(bannerUrl);
        }
        if (request.ThumbHalf is not null)
        {
            var fileName = StorageName.Create(video.Id, nameof(video.ThumbHalf), request.ThumbHalf.Extension);
            var thumbHalfUrl = await storageService.Upload(fileName,
                request.ThumbHalf.FileStream, 
                request.ThumbHalf.ContentType,
                cancellationToken);
            video.UpdateThumbHalf(thumbHalfUrl);
        }
    }

    private static async Task ValidateCastMember(ICastMemberRepository castMemberRepository, UpdateVideoInput request, Domain.Entity.Video video, CancellationToken cancellationToken)
    {
        if (request.CastMembersIds is not null)
        {
            video.RemoveAllCastMembers();
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
    }

    private static async Task ValidateGenres(IGenreRepository genreRepository, UpdateVideoInput request, Domain.Entity.Video video, CancellationToken cancellationToken)
    {
        if (request.GenresIds is not null)
        {
            video.RemoveAllGenres();
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
    }

    private static async Task ValidateCategory(ICategoryRepository categoryRepository, UpdateVideoInput request, Domain.Entity.Video video, CancellationToken cancellationToken)
    {
        if (request.CategoriesIds is not null)
        {
            video.RemoveAllCategories();
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
}

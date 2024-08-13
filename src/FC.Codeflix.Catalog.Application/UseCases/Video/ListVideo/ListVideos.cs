
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.ListVideo;
public class ListVideos : IListVideos
{
    private readonly IVideoRepository _videoRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IGenreRepository _genreRepository;
    private readonly ICastMemberRepository _castMemberRepository;

    public ListVideos(IVideoRepository videoRepository, 
        ICategoryRepository categoryRepository, IGenreRepository genreRepository,
        ICastMemberRepository castMemberRepository)
    {
        _videoRepository = videoRepository;
        _categoryRepository = categoryRepository;
        _genreRepository = genreRepository;
        _castMemberRepository = castMemberRepository;
    }

    public async Task<ListVideoOutput> Handle(ListVideosInput request, CancellationToken cancellationToken)
    {

        var result = await _videoRepository.SearchAsync(
            request.ToSearchInput(), 
            cancellationToken);
        
        IReadOnlyList<DomainEntity.Category>? categories = null;
        var relatedCategoriesIds = result.Items
            .SelectMany(video => video.Categories).Distinct().ToList();
        if (relatedCategoriesIds is not null && relatedCategoriesIds.Count > 0) 
            categories = await _categoryRepository.GetListByIds(relatedCategoriesIds, cancellationToken);

        IReadOnlyList<DomainEntity.Genre>? genres = null;
        var relatedGenresIds = result.Items
            .SelectMany(video => video.Genres).Distinct().ToList();
        if (relatedGenresIds is not null && relatedGenresIds.Count > 0)
            genres = await _genreRepository.GetListByIds(relatedGenresIds, cancellationToken);

        IReadOnlyList<DomainEntity.CastMember>? castMembers = null;
        var relatedCastMemberIds = result.Items
            .SelectMany(video => video.CastMembers).Distinct().ToList();
        if (relatedCastMemberIds is not null && relatedCastMemberIds.Count > 0)
            castMembers = await _castMemberRepository.GetListByIds(relatedCastMemberIds, cancellationToken);

        var output = new ListVideoOutput(result.CurrentPage,
            result.PerPage,
            result.Total,
            result.Items.Select(item => VideoModelOutput.FromVideo(item, 
                categories?.ToList(),
                genres?.ToList(),
                castMembers?.ToList()
                )
            ).ToList());
        return output;
    }
}

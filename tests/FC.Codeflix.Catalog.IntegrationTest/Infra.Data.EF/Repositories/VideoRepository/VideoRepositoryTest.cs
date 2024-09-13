using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;

using Repository = FC.Codeflix.Catalog.Infra.Data.EF.Repositories;

namespace FC.Codeflix.Catalog.IntegrationTest.Infra.Data.EF.Repositories.VideoRepository;

[Collection(nameof(VideoRepositoryTestFixture))]
public class VideoRepositoryTest
{
    private readonly VideoRepositoryTestFixture _fixture;

    public VideoRepositoryTest(VideoRepositoryTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(Insert))]
    [Trait("Integration/Infra.Data", "Repositories - VideoRepository")]
    public async Task Insert()
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleVideo = _fixture.GetExampleVideo();

        var videoRepository = new Repository.VideoRepository(dbContext);

        await videoRepository.Insert(exampleVideo, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var assertsDbContext = _fixture.CreateDbContext(true);
        var dbVideo = await assertsDbContext.Videos.FindAsync(exampleVideo.Id);

        dbVideo.Should().NotBeNull();
        dbVideo!.Id.Should().Be(exampleVideo.Id);
        dbVideo.Title.Should().Be(exampleVideo.Title);
        dbVideo.Description.Should().Be(exampleVideo.Description);
        dbVideo.Opened.Should().Be(exampleVideo.Opened);
        dbVideo.Published.Should().Be(exampleVideo.Published);
        dbVideo.YearLaunched.Should().Be(exampleVideo.YearLaunched);
        dbVideo.Duration.Should().Be(exampleVideo.Duration);
        dbVideo.Rating.Should().Be(exampleVideo.Rating);
        dbVideo.CreatedAt.Should().BeCloseTo(exampleVideo.CreatedAt, TimeSpan.FromSeconds(1));
        dbVideo.Thumb.Should().BeNull();
        dbVideo.ThumbHalf.Should().BeNull();
        dbVideo.Banner.Should().BeNull();
        dbVideo.Media.Should().BeNull();
        dbVideo.Trailer.Should().BeNull();
        dbVideo.Genres.Should().BeEmpty();
        dbVideo.Categories.Should().BeEmpty();
        dbVideo.CastMembers.Should().BeEmpty();
    }

    [Fact(DisplayName = nameof(InsertWithRelations))]
    [Trait("Integration/Infra.Data", "Repositories - VideoRepository")]
    public async Task InsertWithRelations()
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleVideo = _fixture.GetExampleVideo();
        var exampleCastMemberList = _fixture.GetExampleCastMemberList(5);
        var exampleCategoryList = _fixture.GetExampleCategoryList(5);
        var exampleGenreList = _fixture.GetExampleGenreList(5);
        await dbContext.CastMembers.AddRangeAsync(exampleCastMemberList);
        exampleCastMemberList.ForEach(cm => exampleVideo.AddCastMember(cm.Id));
        await dbContext.Categories.AddRangeAsync(exampleCategoryList);
        exampleCategoryList.ForEach(c => exampleVideo.AddCategory(c.Id));
        await dbContext.Genres.AddRangeAsync(exampleGenreList);
        exampleGenreList.ForEach(g => exampleVideo.AddGenre(g.Id));
        await dbContext.SaveChangesAsync();
        var videoRepository = new Repository.VideoRepository(dbContext);

        await videoRepository.Insert(exampleVideo, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var assertsDbContext = _fixture.CreateDbContext(true);
        var dbVideo = await assertsDbContext.Videos.FindAsync(exampleVideo.Id);
        dbVideo.Should().NotBeNull();
        dbVideo!.Genres.Should().BeEmpty();
        dbVideo.Categories.Should().BeEmpty();
        dbVideo.CastMembers.Should().BeEmpty();
        var dbVideosGenres = assertsDbContext.VideosGenres
            .Where(x => x.VideoId == dbVideo!.Id).ToList();
        var dbVideosCategories = assertsDbContext.VideosCategories
            .Where(x => x.VideoId == dbVideo!.Id).ToList();
        var dbVideosCastMembers = assertsDbContext.VideosCastMembers
            .Where(x => x.VideoId == dbVideo!.Id).ToList();
        dbVideosGenres.ForEach(g => exampleGenreList
            .Select(eg => eg.Id).Contains(g.GenreId)
            .Should().BeTrue());
        dbVideosCategories.ForEach(c => exampleCategoryList
            .Select(ec => ec.Id).Contains(c.CategoryId)
            .Should().BeTrue());
        dbVideosCastMembers.ForEach(cm => exampleCastMemberList
            .Select(ecm => ecm.Id).Contains(cm.CastMemberId)
            .Should().BeTrue());
        dbVideosGenres.Count.Should().Be(exampleGenreList.Count);
        dbVideosCategories.Count.Should().Be(exampleVideo.Categories.Count);
        dbVideosCastMembers.Count.Should().Be(exampleVideo.CastMembers.Count);
    }

    [Fact(DisplayName = nameof(InsertWithMediasAndImages))]
    [Trait("Integration/Infra.Data", "Repositories - VideoRepository")]
    public async Task InsertWithMediasAndImages()
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleVideo = _fixture.GetValidVideoWithAllProperties();

        var videoRepository = new Repository.VideoRepository(dbContext);

        await videoRepository.Insert(exampleVideo, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var assertsDbContext = _fixture.CreateDbContext(true);
        var dbVideo = await assertsDbContext.Videos
            .Include(x => x.Media)
            .Include(x => x.Trailer)
            .FirstOrDefaultAsync(x => x.Id == exampleVideo.Id);
        dbVideo.Should().NotBeNull();
        dbVideo!.Id.Should().Be(exampleVideo.Id);
        dbVideo.Thumb.Should().NotBeNull();
        dbVideo.ThumbHalf.Should().NotBeNull();
        dbVideo.Banner.Should().NotBeNull();
        dbVideo.Media.Should().NotBeNull();
        dbVideo.Trailer.Should().NotBeNull();
        dbVideo.Thumb!.Path.Should().Be(exampleVideo.Thumb!.Path);
        dbVideo.ThumbHalf!.Path.Should().Be(exampleVideo.Thumb.Path);
        dbVideo.Banner!.Path.Should().Be(exampleVideo.Thumb.Path);
        dbVideo.Media!.FilePath.Should().Be(exampleVideo.Media!.FilePath);
        dbVideo.Media.EncondedPath.Should().Be(exampleVideo.Media.EncondedPath);
        dbVideo.Media.Status.Should().Be(exampleVideo.Media.Status);
        dbVideo.Trailer!.FilePath.Should().Be(exampleVideo.Trailer!.FilePath);
        dbVideo.Trailer.EncondedPath.Should().Be(exampleVideo.Trailer.EncondedPath);
        dbVideo.Trailer.Status.Should().Be(exampleVideo.Trailer.Status);

    }

    [Fact(DisplayName = nameof(Update))]
    [Trait("Integration/Infra.Data", "Repositories - VideoRepository")]
    public async Task Update()
    {
        var dbContextArrange = _fixture.CreateDbContext();
        var exampleVideo = _fixture.GetExampleVideo();
        await dbContextArrange.AddAsync(exampleVideo);
        await dbContextArrange.SaveChangesAsync();
        var newValues = _fixture.GetExampleVideo();
        var dbContextAct = _fixture.CreateDbContext(true);
        var videoRepository = new Repository.VideoRepository(dbContextAct);
        exampleVideo.Update(newValues.Title, newValues.Description, newValues.Opened,
            newValues.Published, newValues.YearLaunched, newValues.Duration,
            newValues.Rating);

        await videoRepository.Update(exampleVideo, CancellationToken.None);
        await dbContextAct.SaveChangesAsync(CancellationToken.None);

        var assertsDbContext = _fixture.CreateDbContext(true);
        var dbVideo = await assertsDbContext.Videos.FindAsync(exampleVideo.Id);
        dbVideo.Should().NotBeNull();
        dbVideo!.Id.Should().Be(exampleVideo.Id);
        dbVideo.Title.Should().Be(newValues.Title);
        dbVideo.Description.Should().Be(newValues.Description);
        dbVideo.Opened.Should().Be(newValues.Opened);
        dbVideo.Published.Should().Be(newValues.Published);
        dbVideo.YearLaunched.Should().Be(newValues.YearLaunched);
        dbVideo.Duration.Should().Be(newValues.Duration);
        dbVideo.Rating.Should().Be(newValues.Rating);
        dbVideo.CreatedAt.Should().BeCloseTo(newValues.CreatedAt, TimeSpan.FromSeconds(1));
        dbVideo.Thumb.Should().BeNull();
        dbVideo.ThumbHalf.Should().BeNull();
        dbVideo.Banner.Should().BeNull();
        dbVideo.Media.Should().BeNull();
        dbVideo.Trailer.Should().BeNull();
        dbVideo.Genres.Should().BeEmpty();
        dbVideo.Categories.Should().BeEmpty();
        dbVideo.CastMembers.Should().BeEmpty();
    }

    [Fact(DisplayName = nameof(UpdateEntitiesAndValueObjects))]
    [Trait("Integration/Infra.Data", "Repositories - VideoRepository")]
    public async Task UpdateEntitiesAndValueObjects()
    {
        var dbContextArrange = _fixture.CreateDbContext();
        var exampleVideo = _fixture.GetExampleVideo();
        exampleVideo.UpdateTrailer(_fixture.GetValidMediaPath());
        await dbContextArrange.AddAsync(exampleVideo);
        await dbContextArrange.SaveChangesAsync();
        var updatedThumb = _fixture.GetValidImagePath();
        var updatedThumbHalf = _fixture.GetValidImagePath();
        var updatedBanner = _fixture.GetValidImagePath();
        var updatedMedia = _fixture.GetValidMediaPath();
        var updatedMediaEncoded = _fixture.GetValidMediaPath();
        var updatedTrailer = _fixture.GetValidMediaPath();
        var dbContextAct = _fixture.CreateDbContext(true);
        var videoRepository = new Repository.VideoRepository(dbContextAct);
        var savedVideo = dbContextAct.Videos.Single(x => x.Id == exampleVideo.Id);

        savedVideo.UpdateBanner(updatedBanner);
        savedVideo.UpdateThumb(updatedThumb);
        savedVideo.UpdateThumbHalf(updatedThumbHalf);
        savedVideo.UpdateMedia(updatedMedia);
        savedVideo.UpdateAsEncoded(updatedMediaEncoded);
        savedVideo.UpdateTrailer(updatedTrailer);
        await videoRepository.Update(savedVideo, CancellationToken.None);
        dbContextAct.SaveChanges();

        var assertsDbContext = _fixture.CreateDbContext(true);
        var dbVideo = await assertsDbContext.Videos.FindAsync(exampleVideo.Id);
        dbVideo.Should().NotBeNull();
        dbVideo!.Id.Should().Be(savedVideo.Id);
        dbVideo.Thumb.Should().NotBeNull();
        dbVideo.ThumbHalf.Should().NotBeNull();
        dbVideo.Banner.Should().NotBeNull();
        //dbVideo.Trailer.Should().NotBeNull();
        //dbVideo.Media.Should().NotBeNull();
        dbVideo.Thumb!.Path.Should().Be(updatedThumb);
        dbVideo.ThumbHalf!.Path.Should().Be(updatedThumbHalf);
        dbVideo.Banner!.Path.Should().Be(updatedBanner);
        //dbVideo.Media!.FilePath.Should().Be(updatedMedia);
        //dbVideo.Media.EncondedPath.Should().Be(updatedMediaEncoded);
        //dbVideo.Media.Status.Should().Be(MediaStatus.Completed);
        //dbVideo.Trailer!.FilePath.Should().Be(updatedTrailer);

    }

    [Fact(DisplayName = nameof(UpdateWithRelations))]
    [Trait("Integration/Infra.Data", "Repositories - VideoRepository")]
    public async Task UpdateWithRelations()
    {
        var exampleVideo = _fixture.GetExampleVideo();
        var exampleCastMemberList = _fixture.GetExampleCastMemberList(5);
        var exampleCategoryList = _fixture.GetExampleCategoryList(5);
        var exampleGenreList = _fixture.GetExampleGenreList(5);
        using (var dbContext = _fixture.CreateDbContext())
        {
            await dbContext.Videos.AddAsync(exampleVideo);
            await dbContext.CastMembers.AddRangeAsync(exampleCastMemberList);
            await dbContext.Categories.AddRangeAsync(exampleCategoryList);
            await dbContext.Genres.AddRangeAsync(exampleGenreList);
            await dbContext.SaveChangesAsync();
        }

        var actDbContext = _fixture.CreateDbContext(true);
        var savedVideo = actDbContext.Videos.First(v => v.Id == exampleVideo.Id);
        var videoRepository = new Repository.VideoRepository(actDbContext);

        exampleCategoryList.ForEach(c => savedVideo.AddCategory(c.Id));
        exampleCastMemberList.ForEach(cm => savedVideo.AddCastMember(cm.Id));
        exampleGenreList.ForEach(g => savedVideo.AddGenre(g.Id));

        await videoRepository.Update(savedVideo, CancellationToken.None);
        await actDbContext.SaveChangesAsync(CancellationToken.None);

        var assertsDbContext = _fixture.CreateDbContext(true);
        var dbVideo = await assertsDbContext.Videos.FindAsync(exampleVideo.Id);
        dbVideo.Should().NotBeNull();
        dbVideo!.Genres.Should().BeEmpty();
        dbVideo.Categories.Should().BeEmpty();
        dbVideo.CastMembers.Should().BeEmpty();
        var dbVideosGenres = assertsDbContext.VideosGenres
            .Where(x => x.VideoId == dbVideo!.Id).ToList();
        var dbVideosCategories = assertsDbContext.VideosCategories
            .Where(x => x.VideoId == dbVideo!.Id).ToList();
        var dbVideosCastMembers = assertsDbContext.VideosCastMembers
            .Where(x => x.VideoId == dbVideo!.Id).ToList();
        dbVideosGenres.ForEach(g => exampleGenreList
            .Select(eg => eg.Id).Contains(g.GenreId)
            .Should().BeTrue());
        dbVideosCategories.ForEach(c => exampleCategoryList
            .Select(ec => ec.Id).Contains(c.CategoryId)
            .Should().BeTrue());
        dbVideosCastMembers.ForEach(cm => exampleCastMemberList
            .Select(ecm => ecm.Id).Contains(cm.CastMemberId)
            .Should().BeTrue());
        dbVideosGenres.Count.Should().Be(exampleGenreList.Count);
        dbVideosCategories.Count.Should().Be(exampleCategoryList.Count);
        dbVideosCastMembers.Count.Should().Be(exampleCastMemberList.Count);
    }

    [Fact(DisplayName = nameof(Delete))]
    [Trait("Integration/Infra.Data", "Repositories - VideoRepository")]
    public async Task Delete()
    {
        var exampleVideo = _fixture.GetExampleVideo();
        using (var dbContext = _fixture.CreateDbContext())
        {
            await dbContext.Videos.AddAsync(exampleVideo);
            await dbContext.SaveChangesAsync();
        }

        var actDbContext = _fixture.CreateDbContext(true);
        var savedVideo = actDbContext.Videos.First(v => v.Id == exampleVideo.Id);
        var videoRepository = new Repository.VideoRepository(actDbContext);

        await videoRepository.Delete(savedVideo, CancellationToken.None);
        await actDbContext.SaveChangesAsync(CancellationToken.None);

        var assertsDbContext = _fixture.CreateDbContext(true);
        var dbVideo = await assertsDbContext.Videos.FindAsync(exampleVideo.Id);
        dbVideo.Should().BeNull();

    }

    [Fact(DisplayName = nameof(DeleteWithAllPropertiesAndRelations))]
    [Trait("Integration/Infra.Data", "Repositories - VideoRepository")]
    public async Task DeleteWithAllPropertiesAndRelations()
    {
        var exampleVideo = _fixture.GetValidVideoWithAllProperties();
        var exampleCastMemberList = _fixture.GetExampleCastMemberList(5);
        var exampleCategoryList = _fixture.GetExampleCategoryList(5);
        var exampleGenreList = _fixture.GetExampleGenreList(5);
        using (var dbContext = _fixture.CreateDbContext())
        {
            await dbContext.CastMembers.AddRangeAsync(exampleCastMemberList);
            await dbContext.Categories.AddRangeAsync(exampleCategoryList);
            await dbContext.Genres.AddRangeAsync(exampleGenreList);
            exampleCategoryList.ForEach(c =>
                dbContext.VideosCategories.Add(
                    new(c.Id, exampleVideo.Id)
                    )
            );
            exampleCastMemberList.ForEach(cm =>
                dbContext.VideosCastMembers.Add(
                    new(cm.Id, exampleVideo.Id)
                    )
            );
            exampleGenreList.ForEach(g =>
                dbContext.VideosGenres.Add(
                    new(g.Id, exampleVideo.Id)
                    ));
            await dbContext.Videos.AddAsync(exampleVideo);
            await dbContext.SaveChangesAsync();
        }
        var actDbContext = _fixture.CreateDbContext(true);
        var savedVideo = actDbContext.Videos.First(v => v.Id == exampleVideo.Id);
        var videoRepository = new Repository.VideoRepository(actDbContext);

        await videoRepository.Delete(savedVideo, CancellationToken.None);
        await actDbContext.SaveChangesAsync(CancellationToken.None);

        var assertsDbContext = _fixture.CreateDbContext(true);
        var dbVideo = await assertsDbContext.Videos.FindAsync(exampleVideo.Id);
        dbVideo.Should().BeNull();
        assertsDbContext.VideosGenres.Where(x => x.VideoId == exampleVideo!.Id)
            .ToList().Count().Should().Be(0);
        assertsDbContext.VideosCategories
            .Where(x => x.VideoId == exampleVideo!.Id).ToList().Count().Should().Be(0);
        assertsDbContext.VideosCastMembers
            .Where(x => x.VideoId == exampleVideo!.Id).ToList().Count().Should().Be(0);
        assertsDbContext.Set<Media>().Count().Should().Be(0);
    }

    [Fact(DisplayName = nameof(Get))]
    [Trait("Integration/Infra.Data", "Repositories - VideoRepository")]
    public async Task Get()
    {
        var exampleVideo = _fixture.GetExampleVideo();
        using (var dbContext = _fixture.CreateDbContext())
        {
            await dbContext.Videos.AddAsync(exampleVideo);
            await dbContext.SaveChangesAsync();
        }

        var videoRepository = new Repository.VideoRepository(_fixture.CreateDbContext(true));

        var video = await videoRepository.Get(exampleVideo.Id, CancellationToken.None);

        video.Should().NotBeNull();
        video!.Id.Should().Be(exampleVideo.Id);
        video.Thumb.Should().BeNull();
        video.ThumbHalf.Should().BeNull();
        video.Banner.Should().BeNull();
        video.Trailer.Should().BeNull();
        video.Media.Should().BeNull();
        video.Title.Should().Be(exampleVideo.Title);
        video.Description.Should().Be(exampleVideo.Description);
        video.Opened.Should().Be(exampleVideo.Opened);
        video.Published.Should().Be(exampleVideo.Published);
        video.YearLaunched.Should().Be(exampleVideo.YearLaunched);
        video.Duration.Should().Be(exampleVideo.Duration);
        video.Rating.Should().Be(exampleVideo.Rating);
        video.CreatedAt.Should().BeCloseTo(exampleVideo.CreatedAt, TimeSpan.FromSeconds(1));
    }

    [Fact(DisplayName = nameof(GetWithAllProperties))]
    [Trait("Integration/Infra.Data", "Repositories - VideoRepository")]
    public async Task GetWithAllProperties()
    {
        var exampleVideo = _fixture.GetValidVideoWithAllProperties();
        var exampleCastMemberList = _fixture.GetExampleCastMemberList(5);
        var exampleCategoryList = _fixture.GetExampleCategoryList(5);
        var exampleGenreList = _fixture.GetExampleGenreList(5);
        exampleVideo.RemoveAllCastMembers();
        exampleVideo.RemoveAllCategories();
        exampleVideo.RemoveAllGenres();
        using (var dbContext = _fixture.CreateDbContext())
        {
            await dbContext.CastMembers.AddRangeAsync(exampleCastMemberList);
            await dbContext.Categories.AddRangeAsync(exampleCategoryList);
            await dbContext.Genres.AddRangeAsync(exampleGenreList);
            exampleCategoryList.ForEach(c =>
            {
                exampleVideo.AddCategory(c.Id);
                dbContext.VideosCategories.Add(
                    new(c.Id, exampleVideo.Id));
            });
            exampleCastMemberList.ForEach(cm =>
            {
                exampleVideo.AddCastMember(cm.Id);
                dbContext.VideosCastMembers.Add(
                    new(cm.Id, exampleVideo.Id));
            });
            exampleGenreList.ForEach(g =>
            {
                exampleVideo.AddGenre(g.Id);
                dbContext.VideosGenres.Add(
                        new(g.Id, exampleVideo.Id));
            });
            await dbContext.Videos.AddAsync(exampleVideo);
            await dbContext.SaveChangesAsync();
        }

        var videoRepository = new Repository.VideoRepository(_fixture.CreateDbContext(true));

        var video = await videoRepository.Get(exampleVideo.Id, CancellationToken.None);

        video.Should().NotBeNull();
        video!.Id.Should().Be(exampleVideo.Id);
        video.Title.Should().Be(exampleVideo.Title);
        video.Description.Should().Be(exampleVideo.Description);
        video.Opened.Should().Be(exampleVideo.Opened);
        video.Published.Should().Be(exampleVideo.Published);
        video.YearLaunched.Should().Be(exampleVideo.YearLaunched);
        video.Duration.Should().Be(exampleVideo.Duration);
        video.Rating.Should().Be(exampleVideo.Rating);
        video.CreatedAt.Should().BeCloseTo(exampleVideo.CreatedAt, TimeSpan.FromSeconds(1));
        video.Thumb.Should().NotBeNull();
        video.ThumbHalf.Should().NotBeNull();
        video.Banner.Should().NotBeNull();
        video.Media.Should().NotBeNull();
        video.Trailer.Should().NotBeNull();
        video.Thumb!.Path.Should().Be(exampleVideo.Thumb!.Path);
        video.ThumbHalf!.Path.Should().Be(exampleVideo.ThumbHalf!.Path);
        video.Banner!.Path.Should().Be(exampleVideo.Banner!.Path);
        video.Media!.FilePath.Should().Be(exampleVideo.Media!.FilePath);
        video.Media.EncondedPath.Should().Be(exampleVideo.Media.EncondedPath);
        video.Media.Status.Should().Be(exampleVideo.Media.Status);
        video.Trailer!.FilePath.Should().Be(exampleVideo.Trailer!.FilePath);
        video.Genres.Should().BeEquivalentTo(exampleVideo.Genres);
        video.Categories.Should().BeEquivalentTo(exampleVideo.Categories);
        video.CastMembers.Should().BeEquivalentTo(exampleVideo.CastMembers);
    }

    [Fact(DisplayName = nameof(GetThrowIfNotFinded))]
    [Trait("Integration/Infra.Data", "Repositories - VideoRepository")]
    public async Task GetThrowIfNotFinded()
    {

        var videoRepository = new Repository.VideoRepository(_fixture.CreateDbContext());
        var id = Guid.NewGuid();

        var action = () => videoRepository.Get(id, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Video '{id}' not found.");
    }

    [Fact(DisplayName = nameof(Search))]
    [Trait("Integration/Infra.Data", "Repositories - VideoRepository")]
    public async Task Search()
    {
        var videoList = _fixture.GetExampleVideoList();
        using (var arrangeDbContext = _fixture.CreateDbContext())
        {
            await arrangeDbContext.AddRangeAsync(videoList);
            await arrangeDbContext.SaveChangesAsync();
        }
        var actDbContext = _fixture.CreateDbContext(true);
        var videoRepository = new Repository.VideoRepository(actDbContext);
        var searchInput = new SearchInput(1, 20, "", "", default);

        var result = await videoRepository.SearchAsync(searchInput, CancellationToken.None);

        result.Should().NotBeNull();
        result.CurrentPage.Should().Be(searchInput.Page);
        result.PerPage.Should().Be(searchInput.PerPage);
        result.Total.Should().Be(videoList.Count);
        result.Items.Should().NotBeNull();
        result.Items.Should().HaveCount(videoList.Count);
        result.Items.ToList().ForEach(resultItem =>
        {
            var exampleItem = videoList.FirstOrDefault(vl => vl.Id == resultItem.Id);
            videoList.Should().NotBeNull();
            resultItem!.Id.Should().Be(exampleItem!.Id);
            resultItem.Title.Should().Be(exampleItem.Title);
            resultItem.Description.Should().Be(exampleItem.Description);
            resultItem.Opened.Should().Be(exampleItem.Opened);
            resultItem.Published.Should().Be(exampleItem.Published);
            resultItem.YearLaunched.Should().Be(exampleItem.YearLaunched);
            resultItem.Duration.Should().Be(exampleItem.Duration);
            resultItem.Rating.Should().Be(exampleItem.Rating);
            resultItem.CreatedAt.Should().BeCloseTo(exampleItem.CreatedAt, TimeSpan.FromSeconds(1));
            resultItem.Thumb.Should().BeNull();
            resultItem.ThumbHalf.Should().BeNull();
            resultItem.Banner.Should().BeNull();
            resultItem.Media.Should().BeNull();
            resultItem.Trailer.Should().BeNull();
            resultItem.Genres.Should().BeEmpty();
            resultItem.Categories.Should().BeEmpty();
            resultItem.CastMembers.Should().BeEmpty();
        });
    }

    [Fact(DisplayName = nameof(SearchReturnsAllRelations))]
    [Trait("Integration/Infra.Data", "Repositories - VideoRepository")]
    public async Task SearchReturnsAllRelations()
    {
        var videoList = _fixture.GetExampleVideoAllPropertiesList();
        var exampleCastMemberList = _fixture.GetExampleCastMemberList(5);
        var exampleCategoryList = _fixture.GetExampleCategoryList(5);
        var exampleGenreList = _fixture.GetExampleGenreList(5);
        
        using (var dbContext = _fixture.CreateDbContext())
        {
            await dbContext.CastMembers.AddRangeAsync(exampleCastMemberList);
            await dbContext.Categories.AddRangeAsync(exampleCategoryList);
            await dbContext.Genres.AddRangeAsync(exampleGenreList);
            foreach (var exampleVideo in videoList)
            {
                exampleVideo.RemoveAllCastMembers();
                exampleVideo.RemoveAllCategories();
                exampleVideo.RemoveAllGenres();
                exampleCategoryList.ForEach(c =>
                {
                    exampleVideo.AddCategory(c.Id);
                    dbContext.VideosCategories.Add(
                        new(c.Id, exampleVideo.Id));
                });
                exampleCastMemberList.ForEach(cm =>
                {
                    exampleVideo.AddCastMember(cm.Id);
                    dbContext.VideosCastMembers.Add(
                        new(cm.Id, exampleVideo.Id));
                });
                exampleGenreList.ForEach(g =>
                {
                    exampleVideo.AddGenre(g.Id);
                    dbContext.VideosGenres.Add(
                            new(g.Id, exampleVideo.Id));
                });
                await dbContext.Videos.AddAsync(exampleVideo);
            }
            await dbContext.SaveChangesAsync();
        }

        var actDbContext = _fixture.CreateDbContext(true);
        var videoRepository = new Repository.VideoRepository(actDbContext);
        var searchInput = new SearchInput(1, 20, "", "", default);

        var result = await videoRepository.SearchAsync(searchInput, CancellationToken.None);

        result.Should().NotBeNull();
        result.CurrentPage.Should().Be(searchInput.Page);
        result.PerPage.Should().Be(searchInput.PerPage);
        result.Total.Should().Be(videoList.Count);
        result.Items.Should().NotBeNull();
        result.Items.Should().HaveCount(videoList.Count);
        result.Items.ToList().ForEach(resultItem =>
        {
            var exampleItem = videoList.FirstOrDefault(vl => vl.Id == resultItem.Id);
            exampleItem.Should().NotBeNull();
            resultItem!.Id.Should().Be(exampleItem!.Id);
            resultItem.Genres.Should().BeEquivalentTo(exampleItem.Genres);
            resultItem.Categories.Should().BeEquivalentTo(exampleItem.Categories);
            resultItem.CastMembers.Should().BeEquivalentTo(exampleItem.CastMembers);
        });
    }

    [Fact(DisplayName = nameof(SearchReturnsEmpyWhenEmpty))]
    [Trait("Integration/Infra.Data", "Repositories - VideoRepository")]
    public async Task SearchReturnsEmpyWhenEmpty()
    {
        var actDbContext = _fixture.CreateDbContext();
        var videoRepository = new Repository.VideoRepository(actDbContext);
        var searchInput = new SearchInput(1, 20, "", "", default);

        var result = await videoRepository.SearchAsync(searchInput, CancellationToken.None);

        result.Should().NotBeNull();
        result.CurrentPage.Should().Be(searchInput.Page);
        result.PerPage.Should().Be(searchInput.PerPage);
        result.Total.Should().Be(0);
        result.Items.Should().NotBeNull();
        result.Items.Should().HaveCount(0);
    }

    [Theory(DisplayName = nameof(SearchPagination))]
    [Trait("Integration/Infra.Data", "Repositories - VideoRepository")]
    [InlineData(10, 1, 5, 5)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(7, 3, 5, 0)]
    public async Task SearchPagination(int quantityToGenerate, int page, int perPage, int expectedQuantityItems)
    {
        var videoList = _fixture.GetExampleVideoList(quantityToGenerate);
        using (var arrangeDbContext = _fixture.CreateDbContext())
        {
            await arrangeDbContext.AddRangeAsync(videoList);
            await arrangeDbContext.SaveChangesAsync();
        }
        var actDbContext = _fixture.CreateDbContext(true);
        var videoRepository = new Repository.VideoRepository(actDbContext);
        var searchInput = new SearchInput(page, perPage, "", "", default);

        var result = await videoRepository.SearchAsync(searchInput, CancellationToken.None);

        result.Should().NotBeNull();
        result.CurrentPage.Should().Be(searchInput.Page);
        result.PerPage.Should().Be(searchInput.PerPage);
        result.Total.Should().Be(videoList.Count);
        result.Items.Should().NotBeNull();
        result.Items.Should().HaveCount(expectedQuantityItems);
        result.Items.ToList().ForEach(resultItem =>
        {
            var exampleItem = videoList.FirstOrDefault(vl => vl.Id == resultItem.Id);
            videoList.Should().NotBeNull();
            resultItem!.Id.Should().Be(exampleItem!.Id);
            resultItem.Title.Should().Be(exampleItem.Title);
            resultItem.Description.Should().Be(exampleItem.Description);
            
        });
    }
    
    [Theory(DisplayName = nameof(SearchByTitle))]
    [Trait("Integration/Infra.Data", "Repositories - VideoRepository")]
    [InlineData("Action", 1, 5, 1, 1)]
    [InlineData("Horror", 1, 5, 3, 3)]
    [InlineData("Horror", 2, 5, 0, 3)]
    [InlineData("Sci-fi", 1, 5, 4, 4)]
    [InlineData("Sci-fi", 1, 2, 2, 4)]
    [InlineData("Sci-fi", 2, 3, 1, 4)]
    [InlineData("Sci-fi Other", 1, 3, 0, 0)]
    [InlineData("Robots", 1, 5, 2, 2)]
    [InlineData("", 1, 5, 5, 9)]
    public async Task SearchByTitle(string search, int page, int perPage, 
        int expectedQuantityItemsReturned,
        int expectedQuantityTotalItems)
    {
        var videoList = _fixture.GetExampleVideoListByNames(new()
        {
            "Action",
            "Horror",
            "Horror - Robots",
            "Horror - Based on Real Facts",
            "Drama",
            "Sci-fi IA",
            "Sci-fi Space",
            "Sci-fi Robots",
            "Sci-fi Future",
        });
        using (var arrangeDbContext = _fixture.CreateDbContext())
        {
            await arrangeDbContext.AddRangeAsync(videoList);
            await arrangeDbContext.SaveChangesAsync();
        }
        var actDbContext = _fixture.CreateDbContext(true);
        var videoRepository = new Repository.VideoRepository(actDbContext);
        var searchInput = new SearchInput(page, perPage, search, "", default);

        var result = await videoRepository.SearchAsync(searchInput, CancellationToken.None);

        result.Should().NotBeNull();
        result.CurrentPage.Should().Be(searchInput.Page);
        result.PerPage.Should().Be(searchInput.PerPage);
        result.Total.Should().Be(expectedQuantityTotalItems);
        result.Items.Should().NotBeNull();
        result.Items.Should().HaveCount(expectedQuantityItemsReturned);
        result.Items.ToList().ForEach(resultItem =>
        {
            var exampleItem = videoList.FirstOrDefault(vl => vl.Id == resultItem.Id);
            videoList.Should().NotBeNull();
            resultItem!.Id.Should().Be(exampleItem!.Id);
            resultItem.Title.Should().Be(exampleItem.Title);
            resultItem.Description.Should().Be(exampleItem.Description);
            
        });
    }
    [Theory(DisplayName = nameof(SearchOrdered))]
    [Trait("Integration/Infra.Data", "Repositories - VideoRepository")]
    [InlineData("title", "asc")]
    [InlineData("title", "desc")]
    [InlineData("id", "asc")]
    [InlineData("id", "desc")]
    [InlineData("createdAt", "asc")]
    [InlineData("createdAt", "desc")]
    [InlineData("", "desc")]
    public async Task SearchOrdered(string orderBy, string order)
    {
        var videoList = _fixture.GetExampleVideoList(10);
        using (var arrangeDbContext = _fixture.CreateDbContext())
        {
            await arrangeDbContext.AddRangeAsync(videoList);
            await arrangeDbContext.SaveChangesAsync();
        }
        var actDbContext = _fixture.CreateDbContext(true);
        var videoRepository = new Repository.VideoRepository(actDbContext);
        var searchOrder = order == "asc"? SearchOrder.Asc: SearchOrder.Desc;
        var searchInput = new SearchInput(1, 20, "", orderBy, searchOrder);

        var result = await videoRepository.SearchAsync(searchInput, CancellationToken.None);

        result.Should().NotBeNull();
        result.CurrentPage.Should().Be(searchInput.Page);
        result.PerPage.Should().Be(searchInput.PerPage);
        result.Total.Should().Be(10);
        result.Items.Should().NotBeNull();
        result.Items.Should().HaveCount(10);
        var orderedList = _fixture.CloneListOrdered(videoList, searchInput);
        for (int i = 0; i < orderedList.Count; i++)
        {
            var exampleItem = result.Items[i];
            var itemOrdeando = orderedList[i];
            videoList.Should().NotBeNull();
            itemOrdeando!.Id.Should().Be(exampleItem!.Id);
            itemOrdeando.Title.Should().Be(exampleItem.Title);
            itemOrdeando.Description.Should().Be(exampleItem.Description);
        }
    }
}

using FC.Codeflix.Catalog.IntegrationTest.Base;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.IntegrationTest.Applications.UseCases.Genre.Common;

public class GenreUseCasesBaseFixture
    : BaseFixture
{
    public List<Guid> GetRamdoGuids(int? count = null)
        => Enumerable.Range(1, count ?? new Random().Next(1, 10))
            .Select(_ => Guid.NewGuid())
            .ToList();

    public string GetValidGenreName()
        => Faker.Commerce.Categories(1)[0];

    public DomainEntity.Genre GetExampleGenre(bool? isActive = null,
        List<Guid>? categoriesIds = null, string? name = null)
    {
        var genre = new DomainEntity.Genre(name ?? GetValidGenreName(),
        isActive ?? GetRandoBoolean());
        categoriesIds?.ForEach(genre.AddCategory);
        return genre;
    }

    public List<DomainEntity.Genre> GetExampleGenresList(int count = 10, bool gerarCategorias = true)
        => Enumerable.Range(1, count).Select(_ =>
        {
            var genre = new DomainEntity.Genre(GetValidGenreName(),
                GetRandoBoolean());
            if (gerarCategorias) GetRamdoGuids().ForEach(genre.AddCategory);
            return genre;
        }).ToList();

    public List<Domain.Entity.Genre> GetExampleGenresListByNames(List<string> names)
      => names.Select(name => GetExampleGenre(name: name)).ToList();
}

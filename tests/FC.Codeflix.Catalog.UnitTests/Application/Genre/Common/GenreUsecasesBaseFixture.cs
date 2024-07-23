using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.UnitTests.Common;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

using Moq;

namespace FC.Codeflix.Catalog.UnitTests.Application.Genre.Common;
public class GenreUsecasesBaseFixture
    : BaseFixture
{
    public Mock<IGenreRepository> GetRepositoryMock() => new();
    public Mock<IUnitOfWork> GetUnitOfWorkMock() => new();
    public Mock<ICategoryRepository> GetCategoryRepositoryMock() => new();

    public List<Guid> GetRamdoGuids(int? count = null)
        => Enumerable.Range(1, count ?? new Random().Next(1, 10))
            .Select(_ => Guid.NewGuid())
            .ToList();

    public string GetValidGenreName()
        => Faker.Commerce.Categories(1)[0];

    public DomainEntity.Genre GetExampleGenra(bool? isActive = null,
        List<Guid>? categoriesIds = null)
    {
        var genre = new DomainEntity.Genre(GetValidGenreName(),
        isActive ?? GetRandoBoolean());
        categoriesIds?.ForEach(genre.AddCategory);
        return genre;
    }

    public List<DomainEntity.Genre> GetExampleGenresList(int count = 10)
        => Enumerable.Range(1, count).Select(_ =>
        {
            var genre = new DomainEntity.Genre(GetValidGenreName(),
                GetRandoBoolean());
            GetRamdoGuids().ForEach(genre.AddCategory);
            return genre;
        }).ToList();

}

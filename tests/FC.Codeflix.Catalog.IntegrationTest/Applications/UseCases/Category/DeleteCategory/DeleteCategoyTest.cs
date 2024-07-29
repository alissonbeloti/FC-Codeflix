using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;

using UseCase = FC.Codeflix.Catalog.Application.UseCases.Category.DeleteCategory;

namespace FC.Codeflix.Catalog.IntegrationTest.Applications.UseCases.Category.DeleteCategory;

[Collection(nameof(DeleteCategoryTestFixture))]
public class DeleteCategoyTest
{
    private readonly DeleteCategoryTestFixture _fixture;

    public DeleteCategoyTest(DeleteCategoryTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(DeleteCategory))]
    [Trait("Integration/Application", "UseCases - DeleteCategory")]
    private async Task DeleteCategory()
    {
        var dbContext = _fixture.CreateDbContext();
        var categoryExample = _fixture.GetExampleCategory();
        var exampleList = _fixture.GetExampleCategoryList();
        await dbContext.AddRangeAsync(exampleList);
        var tracking = await dbContext.AddAsync(categoryExample);
        await dbContext.SaveChangesAsync();
        tracking.State = EntityState.Detached;
        var repository = new CategoryRepository(dbContext);
        var unitOfWork = new UnitOfWork(dbContext);
        var useCase = new UseCase.DeleteCategory(repository, unitOfWork);
        var input = new UseCase.DeleteCategoryInput(categoryExample.Id);

        await useCase.Handle(input, CancellationToken.None);

        var assertDbContext = _fixture.CreateDbContext(true);
        var dbCategory = await assertDbContext.Categories.FindAsync(categoryExample.Id);
        dbCategory.Should().BeNull();

        var dbCategories = await assertDbContext.Categories.ToListAsync();
        dbCategories.Should().HaveCount(exampleList.Count);
    }

    [Fact(DisplayName = nameof(ThrowWhenCategoryNotFound))]
    [Trait("Integration/Application", "UseCases - DeleteCategory")]
    private async Task ThrowWhenCategoryNotFound()
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleList = _fixture.GetExampleCategoryList();
        await dbContext.AddRangeAsync(exampleList);
        await dbContext.SaveChangesAsync();
        var repository = new CategoryRepository(dbContext);
        var unitOfWork = new UnitOfWork(dbContext);
        var exampleGuid = Guid.NewGuid();
        var input = new UseCase.DeleteCategoryInput(exampleGuid);
        var useCase = new UseCase.DeleteCategory(repository, unitOfWork);

        var task = async () => await useCase.Handle(input, CancellationToken.None);

        await task.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Category '{input.Id}' not found.");
        
    }
}

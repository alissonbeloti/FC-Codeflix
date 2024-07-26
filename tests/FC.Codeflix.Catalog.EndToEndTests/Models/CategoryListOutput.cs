using FC.Codeflix.Catalog.Application.UseCases.Category.Common;

namespace FC.Codeflix.Catalog.EndToEndTests.Models;
public class CategoryListOutput
{
    public TestApiResponseListMeta Meta { get; set; }
    public IReadOnlyList<CategoryModelOutput> Data { get; set; }

    public CategoryListOutput(TestApiResponseListMeta meta, IReadOnlyList<CategoryModelOutput> data)
    {
        Meta = meta;
        Data = data;
    }
}


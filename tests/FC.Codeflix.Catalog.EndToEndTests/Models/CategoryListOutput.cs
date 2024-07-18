using FC.Codeflix.Catalog.Application.UseCases.Category.Common;

namespace FC.Codeflix.Catalog.EndToEndTests.Models;
public class CategoryListOutput
{
    public Meta Meta { get; set; }
    public IReadOnlyList<CategoryModelOutput> Data { get; set; }

    public CategoryListOutput(Meta meta, IReadOnlyList<CategoryModelOutput> data)
    {
        Meta = meta;
        Data = data;
    }
}
public class Meta
{
    public Meta(int currentPage, int perPage, int total)
    {
        CurrentPage = currentPage;
        PerPage = perPage;
        Total = total;
    }

    public int CurrentPage { get; set; }
    public int PerPage { get; set; }
    public int Total { get; set; }
}

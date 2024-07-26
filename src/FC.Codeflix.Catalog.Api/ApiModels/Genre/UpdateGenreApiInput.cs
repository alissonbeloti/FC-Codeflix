namespace FC.Codeflix.Catalog.Api.ApiModels.Genre;

public class UpdateGenreApiInput(string name, bool? isActive, List<Guid>? categoriesIds = null)
{
    public string Name { get; set; } = name;
    public bool? IsActive { get; set; } = isActive;
    public List<Guid>? CategoriesIds { get; set; } = categoriesIds;
}

using MediatR;
using FC.Codeflix.Catalog.Application.UseCases.Category.Common;

namespace FC.Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;
public interface IUpdateCategory : IRequestHandler<UpdateCategoryInput, CategoryModelOutput>
{
    
}

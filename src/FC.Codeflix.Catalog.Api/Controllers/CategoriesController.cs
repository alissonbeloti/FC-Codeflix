using FC.Codeflix.Catalog.Api.ApiModels.Category;
using FC.Codeflix.Catalog.Api.ApiModels.Response;
using FC.Codeflix.Catalog.Api.Authorization;
using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;
using FC.Codeflix.Catalog.Application.UseCases.Category.DeleteCategory;
using FC.Codeflix.Catalog.Application.UseCases.Category.GetCategory;
using FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories;
using FC.Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FC.Codeflix.Catalog.Api.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize(Roles = $"{Roles.Categories},{Roles.Admin}")]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
        => _mediator = mediator;

    
    // GET api/<CategoriesController>/5
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<CategoryModelOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] Guid id, CancellationToken cancellation)
    {
        var output = await _mediator.Send(new GetCategoryInput(id), cancellation);
        return Ok(new ApiResponse<CategoryModelOutput>(output));
    }

    [HttpGet]
    [ProducesResponseType(typeof(CategoryModelOutput), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetList(
        CancellationToken cancellation,
        [FromQuery] int? page = null,
        [FromQuery(Name = "per_page")] int? perPage = null,
        [FromQuery] string? search = null,
        [FromQuery] string? sort = null,
        [FromQuery] SearchOrder? dir = null
        )
    {
        var input = new ListCategoriesInput();
        if (page is not null) input.Page = page.Value;
        if (perPage is not null) input.PerPage = perPage.Value;
        if (!string.IsNullOrWhiteSpace(search)) input.Search = search;
        if (!string.IsNullOrWhiteSpace(sort)) input.Sort = sort;
        if (dir is not null) input.Dir = dir.Value;
        var output = await _mediator.Send(input, cancellation);

        return Ok(new ApiResponseList<CategoryModelOutput>(output));
    }

    // POST api/<CategoriesController>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CategoryModelOutput>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Post([FromBody] CreateCategoryInput input, CancellationToken cancellation)
    {
        var output = await _mediator.Send(input, cancellation);
        return CreatedAtAction(nameof(Post), new { output.Id }, 
            new ApiResponse<CategoryModelOutput>(output));
    }

    // PUT api/<CategoriesController>/5
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CategoryModelOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Put(Guid id, [FromBody] UpdateCategoryApiInput apiInput, CancellationToken cancellationToken)
    {
        var input = new UpdateCategoryInput(id, apiInput.Name, apiInput.Description, apiInput.IsActive);
        var result = await _mediator.Send(input, cancellationToken);
        return Ok(new ApiResponse<CategoryModelOutput>(result));
    }

    // DELETE api/<CategoriesController>/5
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteCategoryInput(id), cancellationToken);
        return NoContent();
    }
}

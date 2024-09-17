using MediatR;
using Microsoft.AspNetCore.Mvc;
using FC.Codeflix.Catalog.Api.ApiModels.Genre;
using FC.Codeflix.Catalog.Api.ApiModels.Response;
using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using FC.Codeflix.Catalog.Application.UseCases.Genre.GetGenre;
using FC.Codeflix.Catalog.Application.UseCases.Genre.DeleteGenre;
using FC.Codeflix.Catalog.Application.UseCases.Genre.CreateGenre;
using FC.Codeflix.Catalog.Application.UseCases.Genre.UpdateGenre;
using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.Application.UseCases.Genre.ListGenres;
using Microsoft.AspNetCore.Authorization;
using FC.Codeflix.Catalog.Api.Authorization;

namespace FC.Codeflix.Catalog.Api.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize(Roles = $"{Roles.Genres},{Roles.Admin}")]
public class GenresController : ControllerBase
{
    private readonly IMediator _mediator;

    public GenresController(IMediator mediator)
        => _mediator = mediator;
    
    // GET api/<CategoriesController>/5
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<GenreModelOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] Guid id, CancellationToken cancellation)
    {
        var output = await _mediator.Send(new GetGenreInput(id), cancellation);
        return Ok(new ApiResponse<GenreModelOutput>(output));
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<GenreModelOutput>), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellation)
    {
        await _mediator.Send(new DeleteGenreInput(id), cancellation);
        return NoContent();
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<GenreModelOutput>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Post([FromBody] CreateGenreInput input, CancellationToken cancellation)
    {
        var output = await _mediator.Send(input, cancellation);
        return CreatedAtAction(
            nameof(Get), 
            new { output.Id },
            new ApiResponse<GenreModelOutput>(output));
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<GenreModelOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdateGenreApiInput input, CancellationToken cancellation)
    {
        var output = await _mediator.Send(new UpdateGenreInput(
            id,
            input.Name,
            input.IsActive,
            input.CategoriesIds
            ), cancellation);
        return Ok(new ApiResponse<GenreModelOutput>(output));
    }

    [HttpGet]
    [ProducesResponseType(typeof(ListGenresOutput), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetList(
        CancellationToken cancellation,
        [FromQuery] int? page = null,
        [FromQuery(Name = "per_page")] int? perPage = null,
        [FromQuery] string? search = null,
        [FromQuery] string? sort = null,
        [FromQuery] SearchOrder? dir = null
        )
    {
        var input = new ListGenresInput();
        if (page is not null) input.Page = page.Value;
        if (perPage is not null) input.PerPage = perPage.Value;
        if (!string.IsNullOrWhiteSpace(search)) input.Search = search;
        if (!string.IsNullOrWhiteSpace(sort)) input.Sort = sort;
        if (dir is not null) input.Dir = dir.Value;
        var output = await _mediator.Send(input, cancellation);

        return Ok(new ApiResponseList<GenreModelOutput>(output));
    }
}

using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;
using FC.Codeflix.Catalog.Application.UseCases.Category.DeleteCategory;
using FC.Codeflix.Catalog.Application.UseCases.Category.GetCategory;
using FC.Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using System;
using System.Threading;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FC.Codeflix.Catalog.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
        => _mediator = mediator;

    
    // GET api/<CategoriesController>/5
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CategoryModelOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] Guid id, CancellationToken cancellation)
    {
        var output = await _mediator.Send(new GetCategoryInput(id), cancellation);
        return Ok(output);
    }

    // POST api/<CategoriesController>
    [HttpPost]
    [ProducesResponseType(typeof(CategoryModelOutput), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Post([FromBody] CreateCategoryInput input, CancellationToken cancellation)
    {
        var output = await _mediator.Send(input, cancellation);
        return CreatedAtAction(nameof(Post), new { output.Id }, output);
    }

    // PUT api/<CategoriesController>/5
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(CategoryModelOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Put(Guid id, [FromBody] UpdateCategoryInput input, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(input, cancellationToken);
        return Ok(result);
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

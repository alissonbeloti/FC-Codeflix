using MediatR;
using Microsoft.AspNetCore.Mvc;
using FC.Codeflix.Catalog.Api.ApiModels.Response;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.Common;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.GetCastMember;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.DeleteCastMember;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.CreateCastMember;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.UpdateCastMember;
using FC.Codeflix.Catalog.Api.ApiModels.CastMembers;
using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.ListCastMembers;

namespace FC.Codeflix.Catalog.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class CastMembersController(IMediator mediator): ControllerBase
{


    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CastMemberModelOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] Guid id, CancellationToken cancellation)
    {
        var output = await mediator.Send(new GetCastMemberInput(id), cancellation);
        return Ok(new ApiResponse<CastMemberModelOutput>(output));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CastMemberModelOutput>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Post([FromBody] CreateCastMemberInput request, CancellationToken cancellation)
    {
        var output = await mediator.Send(request, cancellation);
        return CreatedAtAction(nameof(Get), new { Id = output.Id },
            new ApiResponse<CastMemberModelOutput>(output));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CastMemberModelOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdateCastMemberApiInput request, CancellationToken cancellation)
    {
        var input = new UpdateCastMemberInput(id, request.Name, request.Type);
        var output = await mediator.Send(input, cancellation);
        return Ok(new ApiResponse<CastMemberModelOutput>(output));
    }


    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellation)
    {
        await mediator.Send(new DeleteCastMemberInput(id), cancellation);
        return NoContent();
    }


    [HttpGet]
    [ProducesResponseType(typeof(CastMemberModelOutput), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetList(
        CancellationToken cancellation,
        [FromQuery] int? page = null,
        [FromQuery(Name = "per_page")] int? perPage = null,
        [FromQuery] string? search = null,
        [FromQuery] string? sort = null,
        [FromQuery] SearchOrder? dir = null
        )
    {
        var input = new ListCastMembersInput(
            page ?? 1, 
            perPage ?? 10, 
            search ?? "", 
            sort ?? "", 
            dir ?? SearchOrder.Asc  );
        var output = await mediator.Send(input, cancellation);

        return Ok(new ApiResponseList<CastMemberModelOutput>(output));
    }
}

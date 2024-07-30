using FC.Codeflix.Catalog.Api.ApiModels.Response;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.Common;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.CreateCastMember;
using FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.Common;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc;

using System.Net;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.CreateCastMember;

[Collection(nameof(CastMemberApiBaseFixture))]
public class CreateCastMemberApiTest(CastMemberApiBaseFixture fixture)
{
    [Fact(DisplayName = nameof(Create))]
    [Trait("EndToEnd/API", "CastMember/Create - Endpoints")]
    public async Task Create()
    {
       
        var example = fixture.GetExampleCastMember();
        var input = new CreateCastMemberInput(example.Name, example.Type);

        var (response, output) = await fixture.ApiClient.Post<ApiResponse<CastMemberModelOutput>>(
            $"/castmembers", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.Created);
        output.Should().NotBeNull();
        output!.Data.Id.Should().NotBeEmpty();
        output.Data.Name.Should().Be(input.Name);
        output.Data.Type.Should().Be(input.Type);

        var exampleDb = await fixture.Persistence.GetById(output!.Data.Id);
        exampleDb.Should().NotBeNull();
        exampleDb!.Id.Should().NotBeEmpty();
        exampleDb.Name.Should().Be(input.Name);
        exampleDb.Type.Should().Be(input.Type);
    }

    [Fact(DisplayName = nameof(ThrowWhenNameIsEmpty))]
    [Trait("EndToEnd/API", "CastMember/Create - Endpoints")]
    public async Task ThrowWhenNameIsEmpty()
    {

        var example = fixture.GetExampleCastMember();
        var input = new CreateCastMemberInput("", example.Type);

        var (response, output) = await fixture.ApiClient.Post<ProblemDetails>(
            $"/castmembers", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        output.Should().NotBeNull();
        output!.Title.Should().Be("One or more validation errors occurred");
        output!.Detail.Should().Be("Name should not be empty or null");
    }
}

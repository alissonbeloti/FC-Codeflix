using FC.Codeflix.Catalog.Api.ApiModels.Response;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.Common;
using FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.Common;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc;

using System.Net;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.DeleteCastMember;

[Collection(nameof(CastMemberApiBaseFixture))]
public class DeleteCastMemberApiTest(CastMemberApiBaseFixture fixture)
{
    [Fact(DisplayName = nameof(Delete))]
    [Trait("EndToEnd/API", "CastMember/Delete - Endpoints")]
    public async Task Delete()
    {
        var examples = fixture.GetExampleCastMemberList(5);
        var example = examples[2];
        await fixture.Persistence.InsertList(examples);

        var (response, output) = await fixture.ApiClient.Delete<object>(
            $"/castmembers/{example.Id}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var exampleDb = await fixture.Persistence.GetById(example.Id);
        exampleDb.Should().BeNull();
    }

    [Fact(DisplayName = nameof(NotFound))]
    [Trait("EndToEnd/API", "CastMember/Delete - Endpoints")]
    public async Task NotFound()
    {
        var randomGuid = Guid.NewGuid();
        await fixture.Persistence.InsertList(fixture.GetExampleCastMemberList(5));

        var (response, output) = await fixture.ApiClient.Delete<ProblemDetails>(
            $"/castmembers/{randomGuid}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.NotFound);
        output.Should().NotBeNull();
        output!.Title.Should().Be("Not Found");
        output.Detail.Should().Be($"CastMember '{randomGuid}' not found.");
    }
}

using FC.Codeflix.Catalog.Api.ApiModels.Response;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.Common;
using FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.Common;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc;

using System.Net;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.GetCastMember;

[Collection(nameof(CastMemberApiBaseFixture))]
public class GetCastMemberApiTest(CastMemberApiBaseFixture fixture) : 
    IDisposable
{
    

    [Fact(DisplayName = nameof(Get))]
    [Trait("EndToEnd/API", "CastMember/Get - Endpoints")]
    public async Task Get()
    {
        var examples = fixture.GetExampleCastMemberList(5);
        var example = examples[2];
        await fixture.Persistence.InsertList(examples);

        var (response, output) = await fixture.ApiClient.Get<ApiResponse<CastMemberModelOutput>>(
            $"/castmembers/{example.Id}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Data.Id.Should().Be(example.Id);
        output.Data.Name.Should().Be(example.Name);
        output.Data.Type.Should().Be(example.Type);
    }

    [Fact(DisplayName = nameof(NotFound))]
    [Trait("EndToEnd/API", "CastMember/Get - Endpoints")]
    public async Task NotFound()
    {
        var randomGuid = Guid.NewGuid();
        await fixture.Persistence.InsertList(fixture.GetExampleCastMemberList(5));

        var (response, output) = await fixture.ApiClient.Get<ProblemDetails>(
            $"/castmembers/{randomGuid}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.NotFound);
        output.Should().NotBeNull();
        output!.Title.Should().Be("Not Found");
        output.Detail.Should().Be($"CastMember '{randomGuid}' not found.");
    }
    public void Dispose()
    {
        fixture.CleanPersistence();
    }
}

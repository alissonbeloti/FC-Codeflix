using FC.Codeflix.Catalog.Api.ApiModels.CastMembers;
using FC.Codeflix.Catalog.Api.ApiModels.Response;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.Common;
using FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.Common;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

using System.Net;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.UpdateCastMember;

[Collection(nameof(CastMemberApiBaseFixture))]
public class UpdateCastMemberApiTest(CastMemberApiBaseFixture fixture) : IDisposable
{
    [Fact(DisplayName = nameof(Update))]
    [Trait("EndToEnd/API", "CastMember/Update - Endpoints")]
    public async Task Update()
    {
        var examples = fixture.GetExampleCastMemberList(5);
        var example = examples[2];
        var newName = fixture.GetValidName();
        var newType = fixture.GetRandomCastMemberType();
        await fixture.Persistence.InsertList(examples);

        var (response, output) = await fixture.ApiClient.Put<ApiResponse<CastMemberModelOutput>>(
            $"/castmembers/{example.Id}",
            new UpdateCastMemberApiInput(newName, newType));

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Data.Id.Should().Be(example.Id);
        output.Data.Name.Should().Be(newName);
        output.Data.Type.Should().Be(newType);
        var castMemberFromDb = await fixture.Persistence.GetById(example.Id);
        castMemberFromDb.Should().NotBeNull();
        castMemberFromDb!.Name.Should().Be(newName);
        castMemberFromDb.Type.Should().Be(newType);
        castMemberFromDb.Id.Should().Be(example.Id);
    }

    [Fact(DisplayName = nameof(Returns404NotFound))]
    [Trait("EndToEnd/API", "CastMember/Update - Endpoints")]
    public async Task Returns404NotFound()
    {
        var examples = fixture.GetExampleCastMemberList(5);
        var randomGuid = Guid.NewGuid();
        var newName = fixture.GetValidName();
        var newType = fixture.GetRandomCastMemberType();
        await fixture.Persistence.InsertList(examples);

        var (response, output) = await fixture.ApiClient.Put<ProblemDetails>(
            $"/castmembers/{randomGuid}",
            new UpdateCastMemberApiInput(newName, newType));

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.NotFound);
        output.Should().NotBeNull();
        output!.Status.Should().Be(404);
        output.Detail.Should().Be($"CastMember '{randomGuid}' not found.");
        output.Title.Should().Be($"Not Found");
    }

    [Fact(DisplayName = nameof(Returns422UnprocessableEntity))]
    [Trait("EndToEnd/API", "CastMember/Update - Endpoints")]
    public async Task Returns422UnprocessableEntity()
    {
        var examples = fixture.GetExampleCastMemberList(5);
        var example = examples[2];
        var newName = "";
        var newType = fixture.GetRandomCastMemberType();
        await fixture.Persistence.InsertList(examples);

        var (response, output) = await fixture.ApiClient.Put<ProblemDetails>(
            $"/castmembers/{example.Id}",
            new UpdateCastMemberApiInput(newName, newType));

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        output.Should().NotBeNull();
        output!.Status.Should().Be(422);
        output.Title.Should().Be("One or more validation errors occurred");
        output.Type.Should().Be("UnprocessableEntity");
        output.Detail.Should().Be("Name should not be empty or null");
    }

    public void Dispose() => fixture.CleanPersistence();
}

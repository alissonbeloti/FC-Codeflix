﻿using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.UnitTests.Application.Common;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.CastMember.GetCastMember;
using Moq;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.Common;
using FluentAssertions;
using System.Threading;
using FC.Codeflix.Catalog.Application.Exceptions;

namespace FC.Codeflix.Catalog.UnitTests.Application.CastMember.Get;

[Collection(nameof(GetCastMemberTestFixture))]
public class GetCastMemberTest(GetCastMemberTestFixture fixture)
{
    [Fact(DisplayName = nameof(GetCastMember))]
    [Trait("Application", "GetCastMember - Use Cases")]
    public async Task GetCastMember()
    {
        var repositoryMock = new Mock<ICastMemberRepository>();
        var castMemberExample = fixture.GetExampleCastMember();
        repositoryMock.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(castMemberExample);
        var input = new UseCase.GetCastMemberInput(castMemberExample.Id);
        var useCase = new UseCase.GetCastMember(repositoryMock.Object);

        CastMemberModelOutput output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(castMemberExample.Id);
        output.Name.Should().Be(castMemberExample.Name);
        output.Type.Should().Be(castMemberExample.Type);
        repositoryMock.Verify(x => x.Get(It.Is<Guid>(
            x => x == input.Id
            ), It.IsAny<CancellationToken>()));
    }

    [Fact(DisplayName = nameof(ThrowIfNotFound))]
    [Trait("Application", "GetCastMember - Use Cases")]
    public async Task ThrowIfNotFound()
    {
        var repositoryMock = new Mock<ICastMemberRepository>();
        var castMemberExample = fixture.GetExampleCastMember();
        repositoryMock.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("not found"));
        var input = new UseCase.GetCastMemberInput(Guid.NewGuid());
        var useCase = new UseCase.GetCastMember(repositoryMock.Object);

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        action.Should().NotBeNull();
        await action.Should().ThrowAsync<NotFoundException>();
    }
}

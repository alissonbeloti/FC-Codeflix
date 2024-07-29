using Moq;
using FluentAssertions;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Application.Interfaces;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.CastMember.DeleteCastMember;
using FC.Codeflix.Catalog.Application.Exceptions;

namespace FC.Codeflix.Catalog.UnitTests.Application.CastMember.Delete;

[Collection(nameof(DeleteCastMemberFixture))]
public class DeleteCastMemberTest(DeleteCastMemberFixture fixture)
{
    [Fact(DisplayName = nameof(DeleteCastMember))]
    [Trait("Application", "DeleteCastMember - Use Cases")]
    public async Task DeleteCastMember()
    {
        var repositoryMock = new Mock<ICastMemberRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var castMemberExample = fixture.GetExampleCastMember();
        repositoryMock.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(castMemberExample);
        var input = new UseCase.DeleteCastMemberInput(castMemberExample.Id);
        var useCase = new UseCase.DeleteCastMember(repositoryMock.Object, unitOfWorkMock.Object);

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should().NotThrowAsync();
        repositoryMock.Verify(x => x.Get(It.Is<Guid>(
            x => x == input.Id
            ), It.IsAny<CancellationToken>()));
        repositoryMock.Verify(x => x.Delete(
                It.Is<DomainEntity.CastMember>(y => y.Id == input.Id), 
                It.IsAny<CancellationToken>()), 
            Times.Once
        );
        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
    }

    [Fact(DisplayName = nameof(ThrowsIfNotFound))]
    [Trait("Application", "DeleteCastMember - Use Cases")]
    public async Task ThrowsIfNotFound()
    {
        var repositoryMock = new Mock<ICastMemberRepository>();
        repositoryMock.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("Not Found"));
        var input = new UseCase.DeleteCastMemberInput(Guid.NewGuid());
        var useCase = new UseCase.DeleteCastMember(repositoryMock.Object, Mock.Of<IUnitOfWork>());

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>();
        repositoryMock.Verify(x => x.Get(It.Is<Guid>(
            x => x == input.Id
            ), It.IsAny<CancellationToken>()));
    }
}

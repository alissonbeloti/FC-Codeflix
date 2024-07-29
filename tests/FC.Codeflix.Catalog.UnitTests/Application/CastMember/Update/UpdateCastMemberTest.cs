using Moq;
using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Domain.Repository;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.CastMember.UpdateCastMember;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.Common;
using FluentAssertions;
using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.Exceptions;

namespace FC.Codeflix.Catalog.UnitTests.Application.CastMember.Update;
[Collection(nameof(UpdateCastMemberTestFixture))]
public class UpdateCastMemberTest(UpdateCastMemberTestFixture fixture)
{
    [Fact(DisplayName = nameof(Update))]
    [Trait("Application", "UpdateCastMember - Use Cases")]
    public async Task Update()
    {
        var repositoryMock = new Mock<ICastMemberRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var castMemberExample = fixture.GetExampleCastMember();
        var newName = fixture.GetValidName();
        var newType = fixture.GetRandomCastMemberType();
        repositoryMock.Setup(
            x => x.Get(
                It.Is<Guid>(x => x == castMemberExample.Id),
                It.IsAny<CancellationToken>())
        )
        .ReturnsAsync(castMemberExample);
        var input = new UseCase.UpdateCastMemberInput(castMemberExample.Id, newName, newType);
        var useCase = new UseCase.UpdateCastMember(repositoryMock.Object, unitOfWorkMock.Object);

        CastMemberModelOutput output = await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(
            x => x.Get(
                It.Is<Guid>(x => x == input.Id),
                It.IsAny<CancellationToken>()), 
            Times.Once);
        repositoryMock.Verify(
            x => x.Update(
                It.Is<DomainEntity.CastMember>(x => x.Id == castMemberExample.Id 
                    && x.Name == newName 
                    && x.Type == newType
                ),
                It.IsAny<CancellationToken>()),
            Times.Once);
        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
        output.Should().NotBeNull();
        output.Id.Should().Be(castMemberExample.Id);
        output.Name.Should().Be(newName);
        output.Type.Should().Be(newType);
        output.CreatedAt.Should().Be(castMemberExample.CreatedAt);
    }

    [Fact(DisplayName = nameof(ThrowWhenNotFound))]
    [Trait("Application", "UpdateCastMember - Use Cases")]
    public async Task ThrowWhenNotFound()
    {
        var repositoryMock = new Mock<ICastMemberRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        
        repositoryMock.Setup(
            x => x.Get(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>())
        )
        .ThrowsAsync(new NotFoundException("Not found"));
        var input = new UseCase.UpdateCastMemberInput(Guid.NewGuid(), 
                fixture.GetValidName(), 
                fixture.GetRandomCastMemberType());
        var useCase = new UseCase.UpdateCastMember(repositoryMock.Object, unitOfWorkMock.Object);

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        action.Should().NotBeNull();
        await action.Should().ThrowAsync<NotFoundException>();
    }

    [Fact(DisplayName = nameof(ThrowWhenInvalidName))]
    [Trait("Application", "UpdateCastMember - Use Cases")]
    public async Task ThrowWhenInvalidName()
    {
        var repositoryMock = new Mock<ICastMemberRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var castMemberExample = fixture.GetExampleCastMember();
       
        repositoryMock.Setup(
            x => x.Get(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>())
        )
        .ReturnsAsync(castMemberExample);

        var input = new UseCase.UpdateCastMemberInput(castMemberExample.Id, null, 
            fixture.GetRandomCastMemberType());
        var useCase = new UseCase.UpdateCastMember(repositoryMock.Object, unitOfWorkMock.Object);

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<EntityValidationException>()
            .WithMessage("Name should not be empty or null");
    }
}

using Moq;
using FluentAssertions;
using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Application.Interfaces;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.CastMember.CreateCastMember;
using FC.Codeflix.Catalog.Domain.Exceptions;

namespace FC.Codeflix.Catalog.UnitTests.Application.CastMember.Create;

[Collection(nameof(CreateCastMemberTestFixture))]
public class CreateCastMemberTest(CreateCastMemberTestFixture fixture)
{
    [Fact(DisplayName = nameof(CreateCastMember))]
    [Trait("Application", "CreateCastMember - Use Cases")]
    private async Task CreateCastMember ()
    {
        var input = new UseCase.CreateCastMemberInput(fixture.GetValidName(), 
            fixture.GetRandomCastMemberType());
        var repositoryMock = new Mock<ICastMemberRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var useCase = new UseCase.CreateCastMember(
            repositoryMock.Object,
            unitOfWorkMock.Object);
        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.Name.Should().Be(input.Name);
        output.Type.Should().Be(input.Type);
        output.CreatedAt.Should().NotBeSameDateAs(default);
        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.Verify(x => x.Insert(It.Is<DomainEntity.CastMember>(
            x => (x.Name == input.Name && x.Type == input.Type)
            ), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory(DisplayName = nameof(ThrowsWhenInvalidName))]
    [Trait("Application", "CreateCastMember - Use Cases")]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    private async Task ThrowsWhenInvalidName(string? name)
    {
        var input = new UseCase.CreateCastMemberInput(
            name!,
            fixture.GetRandomCastMemberType());
        var repositoryMock = new Mock<ICastMemberRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var useCase = new UseCase.CreateCastMember(
            repositoryMock.Object,
            unitOfWorkMock.Object);

        var acttion = async () => await useCase.Handle(input, CancellationToken.None);

        acttion.Should().NotBeNull();
        await acttion.Should().ThrowAsync<EntityValidationException>()
            .WithMessage("Name should not be empty or null");
    }
}

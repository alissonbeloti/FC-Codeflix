using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Domain.Exceptions;

using FluentAssertions;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.CastMember;

[Collection(nameof(CastMemberTestFixture))]
public class CastMemberTest(CastMemberTestFixture fixture)
{
    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "CastMember - Aggregates")]
    public void Instantiate()
    {
        var name = fixture.GetValidName();
        var type = fixture.GetRandomCastMemberType();

        var dateTimeBefore = DateTime.Now.AddSeconds(-1);
        DomainEntity.CastMember castMember = new(name, type);
        var dateTimeAfter = DateTime.Now;

        castMember.Id.Should().NotBeEmpty();
        castMember.Name.Should().Be(name);
        castMember.Type.Should().Be(type);
        (castMember.CreatedAt >= dateTimeBefore).Should().BeTrue();
        (castMember.CreatedAt <= dateTimeAfter).Should().BeTrue();
    }

    [Theory(DisplayName = nameof(ThrowErrorWhenNameIsInvalid))]
    [Trait("Domain", "CastMember - Aggregates")]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void ThrowErrorWhenNameIsInvalid(string? name)
    {
        var type = fixture.GetRandomCastMemberType();

        var dateTimeBefore = DateTime.Now.AddSeconds(-1);
        var action = () => new DomainEntity.CastMember(name!, type);
        var dateTimeAfter = DateTime.Now;

        action.Should().NotBeNull();
        action.Should().Throw<EntityValidationException>()
            .WithMessage($"Name should not be empty or null");
    }

    [Fact(DisplayName = nameof(Update))]
    [Trait("Domain", "CastMember - Aggregates")]
    public void Update()
    {
        var newName = fixture.GetValidName();
        var newType = fixture.GetRandomCastMemberType();

        DomainEntity.CastMember castMember = fixture.GetExampleCastMember();
        castMember.Update(newName, newType);

        castMember.Name.Should().Be(newName);
        castMember.Type.Should().Be(newType);
        
    }

    [Theory(DisplayName = nameof(UpdateThrowsErrorWhenNameIsInvalid))]
    [Trait("Domain", "CastMember - Aggregates")]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void UpdateThrowsErrorWhenNameIsInvalid(string? newName)
    {
        
        var newType = fixture.GetRandomCastMemberType();

        DomainEntity.CastMember castMember = fixture.GetExampleCastMember();
        var action = () => castMember.Update(newName!, newType);

        action.Should().Throw<EntityValidationException>()
            .WithMessage($"Name should not be empty or null");

    }
}

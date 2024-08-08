using FC.Codeflix.Catalog.UnitTests.Common;
using FC.Codeflix.Catalog.Domain.ValueObject;
using FluentAssertions;

namespace FC.Codeflix.Catalog.UnitTests.Domain.ValueObject;
public class ImageTest : BaseFixture
{
    
    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Doamin", "Image - ValueObject")]
    public void Instantiate()
    {
        var path = Faker.Image.PicsumUrl();
        var image = new Image(path);

        image.Should().NotBeNull();
        image.Path.Should().Be(path);
    }

    [Fact(DisplayName = nameof(EqualsByProperties))]
    [Trait("Doamin", "Image - ValueObject")]
    public void EqualsByProperties()
    {
        var path = Faker.Image.PicsumUrl();
        var image = new Image(path);
        var sameImage = new Image(path);

        var isItEquals = image == sameImage;

        isItEquals.Should().BeTrue();
    }

    [Fact(DisplayName = nameof(DifferentByProperties))]
    [Trait("Doamin", "Image - ValueObject")]
    public void DifferentByProperties()
    {
        var path = Faker.Image.PicsumUrl();
        var differentPath = Faker.Image.PicsumUrl();
        var image = new Image(path);
        var sameImage = new Image(differentPath);

        var notItEquals = image != sameImage;

        notItEquals.Should().BeTrue();
    }
}

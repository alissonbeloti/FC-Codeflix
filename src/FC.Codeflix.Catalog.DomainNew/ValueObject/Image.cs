
namespace FC.Codeflix.Catalog.Domain.ValueObject;
public class Image(string path) : SeedWork.ValueObject
{
    public string Path { get; } = path;

    public override bool Equals(SeedWork.ValueObject? other)
    {
        return other is Image image 
            && Path == image.Path;
    }

    protected override int GetCustomHashCode() => 
        HashCode.Combine(Path);
}

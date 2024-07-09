namespace FC.Codeflix.Catalog.Domain.SeedWork;
public abstract class Entity
{
    public Guid Id { get; protected set; }
    public Entity() => Id = Guid.NewGuid();
}

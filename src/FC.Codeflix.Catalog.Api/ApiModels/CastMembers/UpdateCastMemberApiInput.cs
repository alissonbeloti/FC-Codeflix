using FC.Codeflix.Catalog.Domain.Enum;

namespace FC.Codeflix.Catalog.Api.ApiModels.CastMembers;

public class UpdateCastMemberApiInput(string name, CastMemberType type)
{
    public string Name { get; private set; } = name;
    public CastMemberType Type { get; private set; } = type;
}

using FC.Codeflix.Catalog.Application.UseCases.CastMember.Common;
using FC.Codeflix.Catalog.Domain.Enum;

using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMember.UpdateCastMember;
public class UpdateCastMemberInput(Guid id, string name, CastMemberType type ) : IRequest<CastMemberModelOutput>
{
    public Guid Id { get; private set; } = id;
    public string Name { get; private set; } = name;
    public CastMemberType Type { get; private set; } = type;
}

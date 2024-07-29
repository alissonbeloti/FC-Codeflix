using MediatR;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.Common;
using FC.Codeflix.Catalog.Domain.Enum;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMember.CreateCastMember;
public class CreateCastMemberInput(string name, CastMemberType type) 
    : IRequest<CastMemberModelOutput>
{
    public string Name { get; private set; } = name;
    public CastMemberType Type { get; private set; } = type;
}

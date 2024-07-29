using MediatR;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.Common;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMember.GetCastMember;
public class GetCastMemberInput(Guid id)
        : IRequest<CastMemberModelOutput>
{
    public Guid Id { get; private set; } = id;
}

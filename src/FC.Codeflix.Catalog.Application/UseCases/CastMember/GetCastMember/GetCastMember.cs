using FC.Codeflix.Catalog.Application.UseCases.CastMember.Common;
using FC.Codeflix.Catalog.Domain.Repository;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMember.GetCastMember;
public class GetCastMember(ICastMemberRepository repository) : IGetCastMember
{
    public async Task<CastMemberModelOutput> Handle(GetCastMemberInput request, CancellationToken cancellationToken)
    {
        var castMember = await repository.Get(request.Id, cancellationToken);
        return CastMemberModelOutput.FromCastMember(castMember);
    }
}

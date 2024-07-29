using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.Common;
using FC.Codeflix.Catalog.Domain.Repository;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMember.UpdateCastMember;
public class UpdateCastMember(ICastMemberRepository repository, IUnitOfWork unitOfWork)
    : IUpdateCastMember
{
    public async Task<CastMemberModelOutput> Handle(UpdateCastMemberInput request, 
        CancellationToken cancellationToken)
    {
        var castMember = await repository.Get(request.Id, cancellationToken);
        castMember.Update(request.Name, request.Type);
        await repository.Update(castMember, cancellationToken);
        await unitOfWork.Commit(cancellationToken);
        return CastMemberModelOutput.FromCastMember(castMember);
    }
}

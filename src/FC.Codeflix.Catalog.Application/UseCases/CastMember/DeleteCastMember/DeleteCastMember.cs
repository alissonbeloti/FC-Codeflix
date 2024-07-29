using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Domain.Repository;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMember.DeleteCastMember;
public class DeleteCastMember(
    ICastMemberRepository castMemberRepository,
    IUnitOfWork unitOfWork)
    : IDeleteCastMember
{
    public async Task Handle(DeleteCastMemberInput request, CancellationToken cancellationToken)
    {
        var castMember = await castMemberRepository.Get(request.Id, cancellationToken);
        await castMemberRepository.Delete(castMember, cancellationToken);
        await unitOfWork.Commit(cancellationToken);
    }
}

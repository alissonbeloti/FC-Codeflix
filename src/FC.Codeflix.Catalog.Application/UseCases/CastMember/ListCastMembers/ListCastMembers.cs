
using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.Common;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMember.ListCastMembers;
public class ListCastMembers(ICastMemberRepository repository) 
    : IListCastMembers
{
    public async Task<ListCastMembersOutput> Handle(ListCastMembersInput request, CancellationToken cancellationToken)
    {
        var searchOutput = await repository.SearchAsync(request.ToSearchInput(), cancellationToken);
        return ListCastMembersOutput.FromSearchOutput(searchOutput);
    }
}

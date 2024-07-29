using MediatR;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.Common;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMember.CreateCastMember;

public interface ICreateCastMember : IRequestHandler<CreateCastMemberInput, CastMemberModelOutput>
{

}

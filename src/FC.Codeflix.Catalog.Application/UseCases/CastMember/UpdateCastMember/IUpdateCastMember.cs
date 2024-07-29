﻿using MediatR;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.Common;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMember.UpdateCastMember;

public interface IUpdateCastMember 
    : IRequestHandler<UpdateCastMemberInput, CastMemberModelOutput>
{
}

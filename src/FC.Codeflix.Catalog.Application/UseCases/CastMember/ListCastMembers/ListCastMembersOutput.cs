﻿using FC.Codeflix.Catalog.Application.Common;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.Common;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMember.ListCastMembers;
public class ListCastMembersOutput : PaginatorListOutput<CastMemberModelOutput>
{
    public ListCastMembersOutput(int page, int perPage, int total, IReadOnlyList<CastMemberModelOutput> items) 
        : base(page, perPage, total, items)
    {
    }

    public static ListCastMembersOutput FromSearchOutput(SearchOutput<DomainEntity.CastMember> searchOutput)
        => new ListCastMembersOutput(
            searchOutput.CurrentPage,
            searchOutput.PerPage,
            searchOutput.Total,
            searchOutput.Items
            .Select(x =>
                CastMemberModelOutput.FromCastMember(x))
                .ToList()
                .AsReadOnly()
            );
}

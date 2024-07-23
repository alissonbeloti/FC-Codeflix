using FC.Codeflix.Catalog.UnitTests.Application.Genre.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FC.Codeflix.Catalog.UnitTests.Application.Genre.Get;

[CollectionDefinition(nameof(GetGrenreTestFixture))]
public class GetGrenreTestFixtureCollection : ICollectionFixture<GetGrenreTestFixture> { }
public class GetGrenreTestFixture : GenreUsecasesBaseFixture
{

}

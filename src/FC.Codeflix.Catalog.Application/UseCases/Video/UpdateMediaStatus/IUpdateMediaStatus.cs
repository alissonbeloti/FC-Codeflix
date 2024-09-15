using FC.Codeflix.Catalog.Application.UseCases.Video.Common;

using MediatR;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.UpdateMediaStatus;
public interface IUpdateMediaStatus
    : IRequestHandler<UpdateMediaStatusInput, VideoModelOutput>
{
}

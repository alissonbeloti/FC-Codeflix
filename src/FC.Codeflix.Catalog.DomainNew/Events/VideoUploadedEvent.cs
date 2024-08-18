using FC.Codeflix.Catalog.Domain.SeedWork;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FC.Codeflix.Catalog.Domain.Events;
public class VideoUploadedEvent : DomainEvent
{
    public Guid ResourceId { get; set; }
    public string FilePath { get; set; }

    public VideoUploadedEvent(Guid resourceId, string filePath)
        : base()
    {
        ResourceId = resourceId;
        FilePath = filePath;
    }
}

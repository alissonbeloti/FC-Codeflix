using FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Models;
public class VideosCastMembers
{
    public Guid CastMemberId { get; set; }
    public CastMember? CastMember { get; set; }
    public Guid VideoId { get; set; }
    public Video? Video { get; set; }

    public VideosCastMembers(Guid castMemberId, Guid videoId)
    {
        CastMemberId = castMemberId;
        VideoId = videoId;
    }
}

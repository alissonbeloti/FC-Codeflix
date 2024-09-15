namespace FC.CodeFlix.Catalog.Infra.Message.DTOs;
public class VideoEncodedMessageDTO
{
    public VideoEncodedMetadataDTO? Video { get; set; }
    public VideoEncodedMetadataDTO? Message { get; set; }
    public string? Error { get; set; }
}

public class VideoEncodedMetadataDTO
{
    public string? EncodedVideoFolder { get; set; }
    public string? FilePath { get; set; }
    public string? ResourceId { get; set; }

    public string FullEncodedVideoFilePath => $"{EncodedVideoFolder}/{FilePath}";
}
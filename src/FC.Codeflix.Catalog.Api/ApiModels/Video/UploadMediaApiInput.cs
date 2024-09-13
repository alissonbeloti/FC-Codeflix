using FC.Codeflix.Catalog.Api.Extensions;
using FC.Codeflix.Catalog.Application.UseCases.Video.UploadMedias;
using FC.Codeflix.Catalog.Domain.Exceptions;

using Microsoft.AspNetCore.Mvc;

namespace FC.Codeflix.Catalog.Api.ApiModels.Video;

public class UploadMediaApiInput
{
    private static class MediaType
    {
        public const string Banner = "banner";
        public const string Thumb = "thumbnail";
        public const string ThumbHalf = "thumbnail_half";
        public const string Media = "video";
        public const string Trailer = "trailer";
    }
    [FromForm(Name = "media_file")]
    public IFormFile Media {  get; set; }

    public UploadMediasInput ToUploadMediaInput(Guid id, string type) =>
    type.ToLower() switch
    {
        MediaType.Banner => new UploadMediasInput(id, Banner: Media.ToFileInput()),
        MediaType.Thumb => new UploadMediasInput(id, Thumb: Media.ToFileInput()),
        MediaType.ThumbHalf => new UploadMediasInput(id, ThumbHalf: Media.ToFileInput()),
        MediaType.Media => new UploadMediasInput(id, VideoInput: Media.ToFileInput()),
        MediaType.Trailer => new UploadMediasInput(id, TrailerInput: Media.ToFileInput()),
        _ => throw new EntityValidationException($"'{type}' is not a valid media type.")
    };
}

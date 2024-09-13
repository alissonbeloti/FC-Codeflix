using FC.Codeflix.Catalog.Application.UseCases.Video.Common;

using System.Runtime.CompilerServices;

namespace FC.Codeflix.Catalog.Api.Extensions;

public static class IFormFileExtensions
{
    public static FileInput? ToFileInput(this  IFormFile? formFile)
    {
        if (formFile == null) return null;
        var fileStream =  new MemoryStream();
        formFile.CopyTo(fileStream);
        return new FileInput(Path.GetExtension(formFile.FileName).Replace(".", ""), fileStream, formFile.ContentType);
    }
}

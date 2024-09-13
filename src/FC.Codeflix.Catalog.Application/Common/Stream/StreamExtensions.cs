namespace FC.Codeflix.Catalog.Application.Common.Stream;
public static class StreamExtensions
{
    public static string ToContentString(this System.IO.Stream stream)
    {
        stream.Seek(0, SeekOrigin.Begin);
        var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}

namespace FC.Codeflix.Catalog.Application.Exceptions;
public abstract class ApplicationException : Exception
{
    protected ApplicationException(string? message) : base(message)
    {
    }

    public static void ThrowIfNull(object? @object, string exceptionMessage)
    {
        if (@object == null)
            throw new NotFoundException(exceptionMessage);
    }
}

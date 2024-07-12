using FC.Codeflix.Catalog.Domain.Exceptions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using System.Net;

namespace FC.Codeflix.Catalog.Api.Filters;

public class ApiGlobalExceptionFilter : IExceptionFilter
{
    private readonly IHostEnvironment _environment;

    public ApiGlobalExceptionFilter(IHostEnvironment environment)
        => _environment = environment;

    public void OnException(ExceptionContext context)
    {
        var details = new ProblemDetails();
        var exception = context.Exception;

        if (_environment.IsDevelopment())
            details.Extensions.Add("StackTrace", exception.StackTrace);

        if (exception is EntityValidationException)
        {
            var ex = exception as EntityValidationException;
            details.Title = "One or more validation errors occurred";
            details.Status = (int)HttpStatusCode.UnprocessableEntity;
            details.Detail = ex!.Message;
            details.Type = "UnprocessableEntity";
        }
        else
        {
            
            details.Title = "An unexpected error occurred";
            details.Status = (int)HttpStatusCode.UnprocessableEntity;
            details.Detail = exception.Message;
            details.Type = "UnexpectedError";
        }
        context.HttpContext.Response.StatusCode = details.Status.Value;
        context.Result = new ObjectResult(details);
        context.ExceptionHandled = true;
    }
}

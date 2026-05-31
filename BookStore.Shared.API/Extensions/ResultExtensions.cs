using BookStore.Shared.Common.Messaging;
using BookStore.Shared.Common.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace BookStore.Shared.API.Extensions;

public static class ResultExtensions
{
    private record struct ErrorInfo(int StatusCode, string Title);

    private static readonly IReadOnlyDictionary<ErrorType, ErrorInfo> ErrorMap =
        new Dictionary<ErrorType, ErrorInfo>
        {
            { ErrorType.Validation, new ErrorInfo(StatusCodes.Status400BadRequest, "Validation Error") },
            { ErrorType.NotFound, new ErrorInfo(StatusCodes.Status404NotFound, "Not Found") },
            { ErrorType.Conflict, new ErrorInfo(StatusCodes.Status409Conflict, "Conflict") },
            { ErrorType.Unauthorized, new ErrorInfo(StatusCodes.Status401Unauthorized, "Unauthorized") },
            { ErrorType.Forbidden, new ErrorInfo(StatusCodes.Status403Forbidden, "Forbidden") }
        };

    public static IActionResult ToActionResult<T>(this Result<T> result, HttpContext httpContext)
    {
        if (result.IsSuccess)
        {
            return new OkObjectResult(result.Value);
        }

        var mapping = ErrorMap.TryGetValue(result.Error.Type, out var info)
            ? info
            : new ErrorInfo(StatusCodes.Status400BadRequest, "Request Failed");

        var messageProvider = httpContext.RequestServices.GetService<IAppMessageProvider>();

        var detail = messageProvider?.GetMessage(result.Error.Code, result.Error.Message)
                     ?? result.Error.Message;

        var problemDetails = new ProblemDetails
        {
            Title = mapping.Title,
            Detail = detail,
            Status = mapping.StatusCode
        };

        problemDetails.Extensions["errorCode"] = result.Error.Code;
        problemDetails.Extensions["errorType"] = result.Error.Type.ToString();

        return new ObjectResult(problemDetails)
        {
            StatusCode = mapping.StatusCode
        };
    }
}
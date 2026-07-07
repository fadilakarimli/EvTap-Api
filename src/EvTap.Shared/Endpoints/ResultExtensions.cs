using EvTap.Shared.Results;
using Microsoft.AspNetCore.Http;
using HttpResults = Microsoft.AspNetCore.Http.Results;

namespace EvTap.Shared.Endpoints;

public static class ResultExtensions
{
    public static IResult ToProblemDetails(this Result result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException("Cannot convert a successful result to a problem.");
        }

        return HttpResults.Problem(
            statusCode: GetStatusCode(result.Error.Type),
            title: GetTitle(result.Error.Type),
            extensions: new Dictionary<string, object?>
            {
                ["errors"] = new[] { result.Error },
            });
    }

    public static IResult ToHttpResult(this Result result) =>
        result.IsSuccess ? HttpResults.NoContent() : result.ToProblemDetails();

    public static IResult ToHttpResult<TValue>(this Result<TValue> result) =>
        result.IsSuccess ? HttpResults.Ok(result.Value) : result.ToProblemDetails();

    public static IResult ToCreatedResult<TValue>(this Result<TValue> result, Func<TValue, string> uriFactory) =>
        result.IsSuccess ? HttpResults.Created(uriFactory(result.Value), result.Value) : result.ToProblemDetails();

    private static int GetStatusCode(ErrorType errorType) => errorType switch
    {
        ErrorType.Validation => StatusCodes.Status400BadRequest,
        ErrorType.NotFound => StatusCodes.Status404NotFound,
        ErrorType.Conflict => StatusCodes.Status409Conflict,
        ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
        ErrorType.Forbidden => StatusCodes.Status403Forbidden,
        _ => StatusCodes.Status500InternalServerError,
    };

    private static string GetTitle(ErrorType errorType) => errorType switch
    {
        ErrorType.Validation => "Bad Request",
        ErrorType.NotFound => "Not Found",
        ErrorType.Conflict => "Conflict",
        ErrorType.Unauthorized => "Unauthorized",
        ErrorType.Forbidden => "Forbidden",
        _ => "Internal Server Error",
    };
}

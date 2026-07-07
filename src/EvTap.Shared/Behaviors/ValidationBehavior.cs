using EvTap.Shared.Results;
using FluentValidation;
using MediatR;

namespace EvTap.Shared.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
        {
            return await next(cancellationToken);
        }

        var failures = validators
            .Select(validator => validator.Validate(request))
            .SelectMany(result => result.Errors)
            .Where(failure => failure is not null)
            .ToList();

        if (failures.Count == 0)
        {
            return await next(cancellationToken);
        }

        var error = Error.Validation(
            "Validation.Failed",
            string.Join(" | ", failures.Select(f => f.ErrorMessage)));

        return CreateValidationResult<TResponse>(error);
    }

    private static TResult CreateValidationResult<TResult>(Error error)
        where TResult : Result
    {
        if (typeof(TResult) == typeof(Result))
        {
            return (TResult)Result.Failure(error);
        }

        var resultType = typeof(TResult).GetGenericArguments()[0];

        var failureMethod = typeof(Result)
            .GetMethod(nameof(Result.Failure), 1, [typeof(Error)])!
            .MakeGenericMethod(resultType);

        return (TResult)failureMethod.Invoke(null, [error])!;
    }
}

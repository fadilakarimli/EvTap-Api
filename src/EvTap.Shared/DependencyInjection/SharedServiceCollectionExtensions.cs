using EvTap.Shared.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace EvTap.Shared.DependencyInjection;

public static class SharedServiceCollectionExtensions
{
    public static IServiceCollection AddSharedPipelineBehaviors(this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}

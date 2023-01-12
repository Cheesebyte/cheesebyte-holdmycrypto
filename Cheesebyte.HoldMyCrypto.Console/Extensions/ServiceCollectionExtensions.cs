using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cheesebyte.HoldMyCrypto.Console.Extensions;

/// <summary>
/// Extension methods for adding functionality to <see cref="IServiceCollection"/>.  
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Duplicates <see cref="AddScoped{TService,TImplementation}"/> with
    /// an optional choice for disabling the call internally. Useful for
    /// hiding optionally injected serviceCollection.
    /// </summary>
    /// <param name="serviceCollection">A <see cref="IServiceCollection"/> object.</param>
    /// <param name="addService">Option to conditionally add <typeparamref name="TImplementation"/>.</param>
    /// <typeparam name="TService">An interface to a service.</typeparam>
    /// <typeparam name="TImplementation">An implementation of a service.</typeparam>
    /// <returns>The input <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddScoped<TService, TImplementation>(
        this IServiceCollection serviceCollection,
        bool addService)
        where TService : class
        where TImplementation : class, TService
    {
        return addService ?
            serviceCollection.AddScoped<TService, TImplementation>() :
            serviceCollection;
    }

    /// <summary>
    /// Adds a FluentValidation validator to a to be bound
    /// <typeparamref name="TOptions"/> object.
    /// </summary>
    /// <param name="serviceCollection">A <see cref="IServiceCollection"/> object.</param>
    /// <param name="config">An <see cref="IConfiguration"/> config.</param>
    /// <param name="sectionName">Section to use from <paramref name="config"/>.</param>
    /// <typeparam name="TOptions">The options object.</typeparam>
    /// <typeparam name="TValidator">The FluentValidator <see cref="IValidator{T}"/> implementation.</typeparam>
    /// <returns>The input <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddValidatedOptions<TOptions, TValidator>(
        this IServiceCollection serviceCollection,
        IConfiguration config,
        string sectionName)
        where TOptions : class
        where TValidator : IValidator<TOptions>, new()
    {
        var configSection = config.GetSection(sectionName);
        serviceCollection
            .AddOptions<TOptions>()
            .Bind(configSection)
            .Validate(options =>
            {
                var validator = new TValidator();
                return validator
                    .Validate(options, strategy => strategy.ThrowOnFailures())
                    .IsValid;
            });

        // Add the validator for runtime use as well
        serviceCollection.AddScoped<IValidator<TOptions>>(_ => new TValidator());
        return serviceCollection;
    }
}
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Soenneker.Cosmos.Container.Abstract;
using Soenneker.Cosmos.Container.Setup.Registrars;

namespace Soenneker.Cosmos.Container.Registrars;

/// <summary>
/// A utility library for storing Azure Cosmos containers
/// </summary>
public static class CosmosContainerUtilRegistrar
{
    public static IServiceCollection AddCosmosContainerUtilAsSingleton(this IServiceCollection services)
    {
        services.AddCosmosContainerSetupUtilAsSingleton().TryAddSingleton<ICosmosContainerUtil, CosmosContainerUtil>();

        return services;
    }
}
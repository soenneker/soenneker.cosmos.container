using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Soenneker.Cosmos.Container.Abstract;

namespace Soenneker.Cosmos.Container.Registrars;

/// <summary>
/// A utility library for storing Azure Cosmos containers
/// </summary>
public static class CosmosContainerUtilRegistrar
{
    /// <summary>
    /// As Singleton
    /// </summary>
    public static void AddCosmosContainerUtil(this IServiceCollection services)
    {
        services.TryAddSingleton<ICosmosContainerUtil, CosmosContainerUtil>();
    }
}

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Soenneker.Cosmos.Client.Registrars;
using Soenneker.Cosmos.Container.Abstract;
using Soenneker.Cosmos.Container.Setup.Registrars;

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
        services.AddCosmosClientUtil();
        services.AddCosmosContainerSetupUtil();
        services.TryAddSingleton<ICosmosContainerUtil, CosmosContainerUtil>();
    }
}
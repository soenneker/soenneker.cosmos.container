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
    public static void AddCosmosContainerUtilAsSingleton(this IServiceCollection services)
    {
        services.AddCosmosContainerSetupUtilAsSingleton();
        services.TryAddSingleton<ICosmosContainerUtil, CosmosContainerUtil>();
    }
}
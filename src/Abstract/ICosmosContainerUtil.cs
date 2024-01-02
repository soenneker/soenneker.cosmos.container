using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Soenneker.Enums.CosmosContainer;

namespace Soenneker.Cosmos.Container.Abstract;

/// <summary>
/// A utility library for storing Azure Cosmos containers <para/>
/// Singleton IoC
/// </summary>
public interface ICosmosContainerUtil : IDisposable, IAsyncDisposable
{
    /// <inheritdoc cref="GetContainer(string)"/>
    [Pure]
    ValueTask<Microsoft.Azure.Cosmos.Container> GetContainer(CosmosContainer container);

    /// <summary>
    /// Implements double check locking mechanism
    /// </summary>
    [Pure]
    ValueTask<Microsoft.Azure.Cosmos.Container> GetContainer(string containerName);

    [Pure]
    ValueTask<Microsoft.Azure.Cosmos.Container> GetContainer(string containerName, CosmosClient cosmosClient, string databaseName);

    /// <inheritdoc cref="DeleteContainer(string)"/>
    ValueTask DeleteContainer(CosmosContainer container);

    ValueTask DeleteContainer(string containerName);
}
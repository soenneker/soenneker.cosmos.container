using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace Soenneker.Cosmos.Container.Abstract;

/// <summary>
/// A utility library for storing Azure Cosmos containers <para/>
/// Singleton IoC
/// </summary>
public interface ICosmosContainerUtil : IDisposable, IAsyncDisposable
{
    [Pure]
    ValueTask<Microsoft.Azure.Cosmos.Container> Get(string endpoint, string accountKey, string databaseName, string containerName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Implements double check locking mechanism
    /// </summary>
    [Pure]
    ValueTask<Microsoft.Azure.Cosmos.Container> Get(string containerName, CancellationToken cancellationToken = default);

    [Pure]
    ValueTask<Microsoft.Azure.Cosmos.Container> Get(string endpoint, string accountKey, string databaseName, string containerName, CosmosClient cosmosClient,
        CancellationToken cancellationToken = default);

    ValueTask Delete(string containerName, CancellationToken cancellationToken = default);

    ValueTask Delete(string endpoint, string accountKey, string databaseName, string containerName, CancellationToken cancellationToken = default);

    [Pure]
    ValueTask<IReadOnlyList<ContainerProperties>> GetAll(CancellationToken cancellationToken = default);

    [Pure]
    ValueTask<IReadOnlyList<ContainerProperties>> GetAll(string endpoint, string accountKey, string databaseName, CancellationToken cancellationToken = default);

    ValueTask DeleteAll(CancellationToken cancellationToken = default);

    ValueTask DeleteAll(string endpoint, string accountKey, string databaseName, CancellationToken cancellationToken = default);
}
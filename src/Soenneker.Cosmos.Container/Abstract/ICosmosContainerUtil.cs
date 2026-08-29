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
    /// <summary>
    /// Returns the configured microsoft.Azure.Cosmos.Container used by the cosmos container.
    /// </summary>
    /// <param name="endpoint">Service endpoint to call.</param>
    /// <param name="accountKey">Account key used for authentication.</param>
    /// <param name="databaseName">Name of the target database.</param>
    /// <param name="containerName">Name of the container to target.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the requested microsoft.Azure.Cosmos.Container.</returns>
    [Pure]
    ValueTask<Microsoft.Azure.Cosmos.Container> Get(string endpoint, string accountKey, string databaseName, string containerName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Implements double check locking mechanism
    /// </summary>
    /// <param name="containerName">Name of the container to target.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the requested microsoft.Azure.Cosmos.Container.</returns>
    [Pure]
    ValueTask<Microsoft.Azure.Cosmos.Container> Get(string containerName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes the entry associated with the specified key.
    /// </summary>
    /// <param name="containerName">Name of the container to target.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes after the targeted files have been deleted.</returns>
    ValueTask Delete(string containerName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes the entry associated with the specified key.
    /// </summary>
    /// <param name="endpoint">Service endpoint to call.</param>
    /// <param name="accountKey">Account key used for authentication.</param>
    /// <param name="databaseName">Name of the target database.</param>
    /// <param name="containerName">Name of the container to target.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes after the targeted files have been deleted.</returns>
    ValueTask Delete(string endpoint, string accountKey, string databaseName, string containerName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the collection returned by get All.</returns>
    [Pure]
    ValueTask<IReadOnlyList<ContainerProperties>> GetAll(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all.
    /// </summary>
    /// <param name="endpoint">Service endpoint to call.</param>
    /// <param name="accountKey">Account key used for authentication.</param>
    /// <param name="databaseName">Name of the target database.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the collection returned by get All.</returns>
    [Pure]
    ValueTask<IReadOnlyList<ContainerProperties>> GetAll(string endpoint, string accountKey, string databaseName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes all.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes after the targeted files have been deleted.</returns>
    ValueTask DeleteAll(CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes all.
    /// </summary>
    /// <param name="endpoint">Service endpoint to call.</param>
    /// <param name="accountKey">Account key used for authentication.</param>
    /// <param name="databaseName">Name of the target database.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes after the targeted files have been deleted.</returns>
    ValueTask DeleteAll(string endpoint, string accountKey, string databaseName, CancellationToken cancellationToken = default);
}

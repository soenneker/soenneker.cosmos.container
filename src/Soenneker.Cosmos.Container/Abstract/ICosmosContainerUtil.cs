using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace Soenneker.Cosmos.Container.Abstract;

/// <summary>
/// Provides cached Azure Cosmos DB container handles and container management operations.
/// </summary>
public interface ICosmosContainerUtil : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Returns a cached container handle for an explicit account and database.
    /// </summary>
    /// <param name="endpoint">Service endpoint to call.</param>
    /// <param name="accountKey">Account key used for authentication.</param>
    /// <param name="databaseName">Name of the target database.</param>
    /// <param name="containerName">Name of the container to target.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the requested container handle.</returns>
    ValueTask<Microsoft.Azure.Cosmos.Container> Get(string endpoint, string accountKey, string databaseName, string containerName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a cached container handle using the configured default account and database.
    /// </summary>
    /// <param name="containerName">Name of the container to target.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the requested container handle.</returns>
    ValueTask<Microsoft.Azure.Cosmos.Container> Get(string containerName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes the entry associated with the specified key.
    /// </summary>
    /// <param name="containerName">Name of the container to target.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes after the container has been deleted.</returns>
    ValueTask Delete(string containerName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes the entry associated with the specified key.
    /// </summary>
    /// <param name="endpoint">Service endpoint to call.</param>
    /// <param name="accountKey">Account key used for authentication.</param>
    /// <param name="databaseName">Name of the target database.</param>
    /// <param name="containerName">Name of the container to target.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes after the container has been deleted.</returns>
    ValueTask Delete(string endpoint, string accountKey, string databaseName, string containerName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists containers in the configured default database.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result contains all container properties.</returns>
    ValueTask<IReadOnlyList<ContainerProperties>> GetAll(CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists containers in an explicit database.
    /// </summary>
    /// <param name="endpoint">Service endpoint to call.</param>
    /// <param name="accountKey">Account key used for authentication.</param>
    /// <param name="databaseName">Name of the target database.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result contains all container properties.</returns>
    ValueTask<IReadOnlyList<ContainerProperties>> GetAll(string endpoint, string accountKey, string databaseName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes every container in the configured default database.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes after all containers have been deleted.</returns>
    ValueTask DeleteAll(CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes every container in an explicit database.
    /// </summary>
    /// <param name="endpoint">Service endpoint to call.</param>
    /// <param name="accountKey">Account key used for authentication.</param>
    /// <param name="databaseName">Name of the target database.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes after all containers have been deleted.</returns>
    ValueTask DeleteAll(string endpoint, string accountKey, string databaseName, CancellationToken cancellationToken = default);
}

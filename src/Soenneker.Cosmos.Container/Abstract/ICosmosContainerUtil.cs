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
    /// Gets the value.
    /// </summary>
    /// <param name="endpoint">The endpoint.</param>
    /// <param name="accountKey">The account key.</param>
    /// <param name="databaseName">The database name.</param>
    /// <param name="containerName">The container name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task containing the result of the operation.</returns>
    [Pure]
    ValueTask<Microsoft.Azure.Cosmos.Container> Get(string endpoint, string accountKey, string databaseName, string containerName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Implements double check locking mechanism
    /// </summary>
    [Pure]
    ValueTask<Microsoft.Azure.Cosmos.Container> Get(string containerName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the delete operation.
    /// </summary>
    /// <param name="containerName">The container name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask Delete(string containerName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the delete operation.
    /// </summary>
    /// <param name="endpoint">The endpoint.</param>
    /// <param name="accountKey">The account key.</param>
    /// <param name="databaseName">The database name.</param>
    /// <param name="containerName">The container name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask Delete(string endpoint, string accountKey, string databaseName, string containerName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task containing the result of the operation.</returns>
    [Pure]
    ValueTask<IReadOnlyList<ContainerProperties>> GetAll(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all.
    /// </summary>
    /// <param name="endpoint">The endpoint.</param>
    /// <param name="accountKey">The account key.</param>
    /// <param name="databaseName">The database name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task containing the result of the operation.</returns>
    [Pure]
    ValueTask<IReadOnlyList<ContainerProperties>> GetAll(string endpoint, string accountKey, string databaseName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes all.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask DeleteAll(CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes all.
    /// </summary>
    /// <param name="endpoint">The endpoint.</param>
    /// <param name="accountKey">The account key.</param>
    /// <param name="databaseName">The database name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask DeleteAll(string endpoint, string accountKey, string databaseName, CancellationToken cancellationToken = default);
}
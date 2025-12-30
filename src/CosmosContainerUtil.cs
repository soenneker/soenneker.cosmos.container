using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Soenneker.Cosmos.Client.Abstract;
using Soenneker.Cosmos.Container.Abstract;
using Soenneker.Cosmos.Container.Setup.Abstract;
using Soenneker.Cosmos.Database.Abstract;
using Soenneker.Extensions.Configuration;
using Soenneker.Extensions.Task;
using Soenneker.Extensions.ValueTask;
using Soenneker.Utils.SingletonDictionary;

namespace Soenneker.Cosmos.Container;

/// <inheritdoc cref="ICosmosContainerUtil"/>
public sealed class CosmosContainerUtil : ICosmosContainerUtil
{
    private readonly ILogger<CosmosContainerUtil> _logger;
    private readonly IConfiguration _config;
    private readonly ICosmosDatabaseUtil _databaseUtil;

    private readonly SingletonDictionary<Microsoft.Azure.Cosmos.Container, CosmosContainerArgs> _containers;

    public CosmosContainerUtil(IConfiguration config, ILogger<CosmosContainerUtil> logger, ICosmosClientUtil cosmosClientUtil,
        ICosmosContainerSetupUtil cosmosContainerSetupUtil, ICosmosDatabaseUtil databaseUtil)
    {
        _logger = logger;
        _config = config;
        _databaseUtil = databaseUtil;

        _containers = new SingletonDictionary<Microsoft.Azure.Cosmos.Container, CosmosContainerArgs>(async (key, cancellationToken, args) =>
        {
            CosmosClient client = await cosmosClientUtil.Get(args.Endpoint, args.AccountKey, cancellationToken)
                                                        .NoSync();

            bool ensureContainerOnFirstUse = config.GetValue("Azure:Cosmos:EnsureContainerOnFirstUse", true);

            if (ensureContainerOnFirstUse)
            {
                _ = await cosmosContainerSetupUtil.Ensure(args.Endpoint, args.AccountKey, args.DatabaseName, args.ContainerName, cancellationToken)
                                                  .NoSync();
            }

            return client.GetContainer(args.DatabaseName, args.ContainerName);
        });
    }

    public ValueTask<Microsoft.Azure.Cosmos.Container> Get(string endpoint, string accountKey, string databaseName, string containerName,
        CancellationToken cancellationToken = default)
    {
        string key = string.Concat(endpoint, "-", databaseName, "-", containerName);

        var args = new CosmosContainerArgs(endpoint, accountKey, databaseName, containerName);

        return _containers.Get(key, args, cancellationToken);
    }

    public ValueTask<Microsoft.Azure.Cosmos.Container> Get(string containerName, CancellationToken cancellationToken = default)
    {
        var endpoint = _config.GetValueStrict<string>("Azure:Cosmos:Endpoint");
        var accountKey = _config.GetValueStrict<string>("Azure:Cosmos:AccountKey");
        var databaseName = _config.GetValueStrict<string>("Azure:Cosmos:DatabaseName");

        return Get(endpoint, accountKey, databaseName, containerName, cancellationToken);
    }

    public ValueTask<Microsoft.Azure.Cosmos.Container> Get(string endpoint, string accountKey, string databaseName, string containerName,
        CosmosClient cosmosClient, CancellationToken cancellationToken = default)
    {
        Microsoft.Azure.Cosmos.Container container = cosmosClient.GetContainer(databaseName, containerName);
        return ValueTask.FromResult(container);
    }

    public async ValueTask Delete(string endpoint, string accountKey, string databaseName, string containerName, CancellationToken cancellationToken = default)
    {
        _logger.LogCritical("Deleting container {container} in {database}! ...", containerName, databaseName);

        Microsoft.Azure.Cosmos.Container container = await Get(endpoint, accountKey, databaseName, containerName, cancellationToken)
            .NoSync();
        await container.DeleteContainerAsync(cancellationToken: cancellationToken)
                       .NoSync();

        string key = string.Concat(endpoint, "-", databaseName, "-", containerName);

        await _containers.Remove(key, cancellationToken)
                         .NoSync();

        _logger.LogWarning("Finished deleting container {container} in {database}", containerName, databaseName);
    }

    public ValueTask Delete(string containerName, CancellationToken cancellationToken = default)
    {
        var endpoint = _config.GetValueStrict<string>("Azure:Cosmos:Endpoint");
        var accountKey = _config.GetValueStrict<string>("Azure:Cosmos:AccountKey");
        var databaseName = _config.GetValueStrict<string>("Azure:Cosmos:DatabaseName");

        return Delete(endpoint, accountKey, databaseName, containerName, cancellationToken);
    }

    public ValueTask<IReadOnlyList<ContainerProperties>> GetAll(CancellationToken cancellationToken = default)
    {
        var endpoint = _config.GetValueStrict<string>("Azure:Cosmos:Endpoint");
        var accountKey = _config.GetValueStrict<string>("Azure:Cosmos:AccountKey");
        var databaseName = _config.GetValueStrict<string>("Azure:Cosmos:DatabaseName");

        return GetAll(endpoint, accountKey, databaseName, cancellationToken);
    }

    public async ValueTask<IReadOnlyList<ContainerProperties>> GetAll(string endpoint, string accountKey, string databaseName,
        CancellationToken cancellationToken = default)
    {
        Microsoft.Azure.Cosmos.Database database = await _databaseUtil.Get(endpoint, accountKey, databaseName, cancellationToken)
                                                                      .NoSync();

        var containers = new List<ContainerProperties>();
        using FeedIterator<ContainerProperties>? iterator = database.GetContainerQueryIterator<ContainerProperties>();

        while (iterator.HasMoreResults)
        {
            foreach (ContainerProperties props in await iterator.ReadNextAsync(cancellationToken)
                                                                .NoSync())
            {
                containers.Add(props);
            }
        }

        return containers;
    }

    public ValueTask DeleteAll(CancellationToken cancellationToken = default)
    {
        var endpoint = _config.GetValueStrict<string>("Azure:Cosmos:Endpoint");
        var accountKey = _config.GetValueStrict<string>("Azure:Cosmos:AccountKey");
        var databaseName = _config.GetValueStrict<string>("Azure:Cosmos:DatabaseName");

        return DeleteAll(endpoint, accountKey, databaseName, cancellationToken);
    }

    public async ValueTask DeleteAll(string endpoint, string accountKey, string databaseName, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<ContainerProperties> containers = await GetAll(endpoint, accountKey, databaseName, cancellationToken)
            .NoSync();

        foreach (ContainerProperties props in containers)
        {
            await Delete(endpoint, accountKey, databaseName, props.Id, cancellationToken)
                .NoSync();
        }
    }

    public void Dispose()
    {
        _containers.Dispose();
    }

    public ValueTask DisposeAsync()
    {
        return _containers.DisposeAsync();
    }

    public readonly record struct CosmosContainerArgs(string Endpoint, string AccountKey, string DatabaseName, string ContainerName);
}
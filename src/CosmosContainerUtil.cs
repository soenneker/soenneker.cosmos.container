using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Soenneker.Cosmos.Client.Abstract;
using Soenneker.Cosmos.Container.Abstract;
using Soenneker.Cosmos.Container.Setup.Abstract;
using Soenneker.Cosmos.Container.Utils;
using Soenneker.Cosmos.Database.Abstract;
using Soenneker.Dictionaries.SingletonKeys;
using Soenneker.Extensions.Configuration;
using Soenneker.Extensions.Task;
using Soenneker.Extensions.ValueTask;

namespace Soenneker.Cosmos.Container;

/// <inheritdoc cref="ICosmosContainerUtil"/>
public sealed class CosmosContainerUtil : ICosmosContainerUtil
{
    private readonly ILogger<CosmosContainerUtil> _logger;
    private readonly ICosmosDatabaseUtil _databaseUtil;
    private readonly ICosmosClientUtil _cosmosClientUtil;
    private readonly ICosmosContainerSetupUtil _cosmosContainerSetupUtil;

    private readonly bool _ensureContainerOnFirstUse;

    private readonly string _defaultEndpoint;
    private readonly string _defaultAccountKey;
    private readonly string _defaultDatabaseName;

    // 🔑 VALUE-TYPE KEY — no allocations per lookup
    private readonly SingletonKeyDictionary<CosmosContainerKey, Microsoft.Azure.Cosmos.Container, CosmosContainerArgs> _containers;

    public CosmosContainerUtil(IConfiguration config, ILogger<CosmosContainerUtil> logger, ICosmosClientUtil cosmosClientUtil,
        ICosmosContainerSetupUtil cosmosContainerSetupUtil, ICosmosDatabaseUtil databaseUtil)
    {
        _logger = logger;
        _databaseUtil = databaseUtil;
        _cosmosClientUtil = cosmosClientUtil;
        _cosmosContainerSetupUtil = cosmosContainerSetupUtil;

        _ensureContainerOnFirstUse = config.GetValue("Azure:Cosmos:EnsureContainerOnFirstUse", true);

        _defaultEndpoint = config.GetValueStrict<string>("Azure:Cosmos:Endpoint");
        _defaultAccountKey = config.GetValueStrict<string>("Azure:Cosmos:AccountKey");
        _defaultDatabaseName = config.GetValueStrict<string>("Azure:Cosmos:DatabaseName");

        // method group → no closure
        _containers = new SingletonKeyDictionary<CosmosContainerKey, Microsoft.Azure.Cosmos.Container, CosmosContainerArgs>(CreateContainer);
    }

    private async ValueTask<Microsoft.Azure.Cosmos.Container> CreateContainer(CosmosContainerKey key, CosmosContainerArgs args,
        CancellationToken cancellationToken)
    {
        CosmosClient client = await _cosmosClientUtil.Get(args.Endpoint, args.AccountKey, cancellationToken)
                                                     .NoSync();

        if (_ensureContainerOnFirstUse)
        {
            await _cosmosContainerSetupUtil.Ensure(args.Endpoint, args.AccountKey, args.DatabaseName, args.ContainerName, cancellationToken)
                                           .NoSync();
        }

        return client.GetContainer(args.DatabaseName, args.ContainerName);
    }

    public ValueTask<Microsoft.Azure.Cosmos.Container> Get(string endpoint, string accountKey, string databaseName, string containerName,
        CancellationToken cancellationToken = default)
    {
        var key = new CosmosContainerKey(endpoint, databaseName, containerName);
        var args = new CosmosContainerArgs(endpoint, accountKey, databaseName, containerName);

        return _containers.Get(key, args, cancellationToken);
    }

    public ValueTask<Microsoft.Azure.Cosmos.Container> Get(string containerName, CancellationToken cancellationToken = default)
    {
        return Get(_defaultEndpoint, _defaultAccountKey, _defaultDatabaseName, containerName, cancellationToken);
    }

    public async ValueTask Delete(string endpoint, string accountKey, string databaseName, string containerName, CancellationToken cancellationToken = default)
    {
        _logger.LogCritical("Deleting container {container} in {database}! ...", containerName, databaseName);

        var key = new CosmosContainerKey(endpoint, databaseName, containerName);
        var args = new CosmosContainerArgs(endpoint, accountKey, databaseName, containerName);

        Microsoft.Azure.Cosmos.Container container = await _containers.Get(key, args, cancellationToken)
                                                                      .NoSync();

        await container.DeleteContainerAsync(cancellationToken: cancellationToken)
                       .NoSync();
        await _containers.Remove(key, cancellationToken)
                         .NoSync();

        _logger.LogWarning("Finished deleting container {container} in {database}", containerName, databaseName);
    }

    public ValueTask Delete(string containerName, CancellationToken cancellationToken = default)
    {
        return Delete(_defaultEndpoint, _defaultAccountKey, _defaultDatabaseName, containerName, cancellationToken);
    }

    public ValueTask<IReadOnlyList<ContainerProperties>> GetAll(CancellationToken cancellationToken = default)
    {
        return GetAll(_defaultEndpoint, _defaultAccountKey, _defaultDatabaseName, cancellationToken);
    }

    public async ValueTask<IReadOnlyList<ContainerProperties>> GetAll(string endpoint, string accountKey, string databaseName,
        CancellationToken cancellationToken = default)
    {
        Microsoft.Azure.Cosmos.Database database = await _databaseUtil.Get(endpoint, accountKey, databaseName, cancellationToken)
                                                                      .NoSync();

        var containers = new List<ContainerProperties>();
        using FeedIterator<ContainerProperties> iterator = database.GetContainerQueryIterator<ContainerProperties>();

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
        return DeleteAll(_defaultEndpoint, _defaultAccountKey, _defaultDatabaseName, cancellationToken);
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

    public void Dispose() => _containers.Dispose();

    public ValueTask DisposeAsync() => _containers.DisposeAsync();
}
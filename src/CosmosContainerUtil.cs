using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Soenneker.Cosmos.Client.Abstract;
using Soenneker.Cosmos.Container.Abstract;
using Soenneker.Cosmos.Container.Setup.Abstract;
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
    private readonly SingletonDictionary<Microsoft.Azure.Cosmos.Container> _containers;

    public CosmosContainerUtil(ICosmosClientUtil cosmosClientUtil, ICosmosContainerSetupUtil cosmosContainerSetupUtil, IConfiguration config,
        ILogger<CosmosContainerUtil> logger)
    {
        _logger = logger;
        _config = config;

        _containers = new SingletonDictionary<Microsoft.Azure.Cosmos.Container>(async (key, cancellationToken, args) =>
        {
            CosmosClient client = await cosmosClientUtil.Get(cancellationToken).NoSync();

            var databaseName = (string)args[0];
            var containerName = (string)args[1];

            var ensureContainerOnFirstUse = config.GetValue<bool?>("Azure:Cosmos:EnsureContainerOnFirstUse");

            if (ensureContainerOnFirstUse == null || ensureContainerOnFirstUse.Value)
            {
                _ = await cosmosContainerSetupUtil.Ensure(databaseName, containerName, cancellationToken).NoSync();
            }

            return client.GetContainer(databaseName, containerName);
        });
    }

    public ValueTask<Microsoft.Azure.Cosmos.Container> Get(string databaseName, string containerName, CancellationToken cancellationToken = default)
    {
        var key = $"{databaseName}-{containerName}";

        return _containers.Get(key, cancellationToken, databaseName, containerName);
    }

    public ValueTask<Microsoft.Azure.Cosmos.Container> Get(string containerName, CancellationToken cancellationToken = default)
    {
        var databaseName = _config.GetValueStrict<string>("Azure:Cosmos:DatabaseName");

        return Get(databaseName, containerName, cancellationToken);
    }

    public ValueTask<Microsoft.Azure.Cosmos.Container> Get(string databaseName, string containerName, CosmosClient cosmosClient,
        CancellationToken cancellationToken = default)
    {
        int hashOfClient = cosmosClient.GetHashCode();

        var key = $"{databaseName}-{containerName}-{hashOfClient}";

        return _containers.Get(key, cancellationToken, databaseName, containerName);
    }

    public async ValueTask Delete(string databaseName, string containerName, CancellationToken cancellationToken = default)
    {
        _logger.LogCritical("Deleting container {container} in {database}! ...", containerName, databaseName);

        Microsoft.Azure.Cosmos.Container container = await Get(databaseName, containerName, cancellationToken).NoSync();
        await container.DeleteContainerAsync(cancellationToken: cancellationToken).NoSync();

        var key = $"{databaseName}-{containerName}";

        await _containers.Remove(key, cancellationToken).NoSync();

        _logger.LogWarning("Finished deleting container {container} in {database}", containerName, databaseName);
    }

    public ValueTask Delete(string containerName, CancellationToken cancellationToken = default)
    {
        var databaseName = _config.GetValueStrict<string>("Azure:Cosmos:DatabaseName");

        return Delete(databaseName, containerName, cancellationToken);
    }

    public void Dispose()
    {
        _containers.Dispose();
    }

    public ValueTask DisposeAsync()
    {
        return _containers.DisposeAsync();
    }
}
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
            var endpoint = (string)args[0];
            var accountKey = (string)args[1];
            var databaseName = (string)args[2];
            var containerName = (string)args[3];

            CosmosClient client = await cosmosClientUtil.Get(endpoint, accountKey, cancellationToken).NoSync();

            bool ensureContainerOnFirstUse = config.GetValue("Azure:Cosmos:EnsureContainerOnFirstUse", true);

            if (ensureContainerOnFirstUse)
            {
                _ = await cosmosContainerSetupUtil.Ensure(endpoint, accountKey, databaseName, containerName, cancellationToken).NoSync();
            }

            return client.GetContainer(databaseName, containerName);
        });
    }

    public ValueTask<Microsoft.Azure.Cosmos.Container> Get(string endpoint, string accountKey, string databaseName, string containerName, CancellationToken cancellationToken = default)
    {
        var key = $"{endpoint}-{databaseName}-{containerName}";

        return _containers.Get(key, cancellationToken,endpoint, accountKey, databaseName, containerName);
    }

    public ValueTask<Microsoft.Azure.Cosmos.Container> Get(string containerName, CancellationToken cancellationToken = default)
    {
        var endpoint = _config.GetValueStrict<string>("Azure:Cosmos:Endpoint");
        var accountKey = _config.GetValueStrict<string>("Azure:Cosmos:AccountKey");
        var databaseName = _config.GetValueStrict<string>("Azure:Cosmos:DatabaseName");

        return Get(endpoint, accountKey, databaseName, containerName, cancellationToken);
    }

    public ValueTask<Microsoft.Azure.Cosmos.Container> Get(string endpoint, string accountKey, string databaseName, string containerName, CosmosClient cosmosClient,
        CancellationToken cancellationToken = default)
    {
        int hashOfClient = cosmosClient.GetHashCode();

        var key = $"{endpoint}-{databaseName}-{containerName}-{hashOfClient}";

        return _containers.Get(key, cancellationToken, databaseName, containerName);
    }

    public async ValueTask Delete(string endpoint, string accountKey, string databaseName, string containerName, CancellationToken cancellationToken = default)
    {
        _logger.LogCritical("Deleting container {container} in {database}! ...", containerName, databaseName);

        Microsoft.Azure.Cosmos.Container container = await Get(endpoint, accountKey, databaseName, containerName, cancellationToken).NoSync();
        await container.DeleteContainerAsync(cancellationToken: cancellationToken).NoSync();

        var key = $"{endpoint}-{databaseName}-{containerName}";

        await _containers.Remove(key, cancellationToken).NoSync();

        _logger.LogWarning("Finished deleting container {container} in {database}", containerName, databaseName);
    }

    public ValueTask Delete(string containerName, CancellationToken cancellationToken = default)
    {
        var endpoint = _config.GetValueStrict<string>("Azure:Cosmos:Endpoint");
        var accountKey = _config.GetValueStrict<string>("Azure:Cosmos:AccountKey");
        var databaseName = _config.GetValueStrict<string>("Azure:Cosmos:DatabaseName");

        return Delete(endpoint, accountKey, databaseName, containerName, cancellationToken);
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
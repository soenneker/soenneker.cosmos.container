using System;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Soenneker.Cosmos.Client.Abstract;
using Soenneker.Cosmos.Container.Abstract;
using Soenneker.Cosmos.Container.Setup.Abstract;
using Soenneker.Extensions.Task;
using Soenneker.Extensions.ValueTask;
using Soenneker.Utils.SingletonDictionary;

namespace Soenneker.Cosmos.Container;

/// <inheritdoc cref="ICosmosContainerUtil"/>
public class CosmosContainerUtil : ICosmosContainerUtil
{
    private readonly ILogger<CosmosContainerUtil> _logger;

    private readonly SingletonDictionary<Microsoft.Azure.Cosmos.Container> _containers;

    private bool _ensureContainerOnFirstUse;
    private string? _databaseName;

    public CosmosContainerUtil(ICosmosClientUtil cosmosClientUtil, ICosmosContainerSetupUtil cosmosContainerSetupUtil, IConfiguration config, ILogger<CosmosContainerUtil> logger)
    {
        _logger = logger;

        SetConfiguration(config);

        _containers = new SingletonDictionary<Microsoft.Azure.Cosmos.Container>(async args =>
        {
            CosmosClient client = await cosmosClientUtil.Get().NoSync();

            var databaseName = (string)args![0];
            var containerName = (string)args[1];

            if (_ensureContainerOnFirstUse)
                _ = await cosmosContainerSetupUtil.Ensure(containerName).NoSync();

            Microsoft.Azure.Cosmos.Container container = client.GetContainer(databaseName, containerName);

            return container;
        });
    }

    private void SetConfiguration(IConfiguration config)
    {
        _ensureContainerOnFirstUse = config.GetValue<bool>("Azure:Cosmos:EnsureContainerOnFirstUse");
        _databaseName = config.GetValue<string>("Azure:Cosmos:DatabaseName");

        if (_databaseName == null)
            throw new Exception("Azure:Cosmos:DatabaseName is required");
    }

    public ValueTask<Microsoft.Azure.Cosmos.Container> Get(string containerName)
    {
        return _containers.Get(containerName, _databaseName!, containerName);
    }

    public ValueTask<Microsoft.Azure.Cosmos.Container> Get(string containerName, CosmosClient cosmosClient, string databaseName)
    {
        int hashOfClient = cosmosClient.GetHashCode();

        var containerKey = $"{containerName}-{hashOfClient}";

        return _containers.Get(containerKey, databaseName, containerName);
    }

    public async ValueTask Delete(string containerName)
    {
        _logger.LogCritical("Deleting container {container}! ...", containerName);

        Microsoft.Azure.Cosmos.Container container = await Get(containerName).NoSync();
        await container.DeleteContainerAsync().NoSync();

        await _containers.Remove(containerName).NoSync();

        _logger.LogWarning("Finished deleting container {container}", containerName);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        _containers.Dispose();
    }

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        return _containers.DisposeAsync();
    }
}
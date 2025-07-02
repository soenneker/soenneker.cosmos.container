﻿using System;
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

    private readonly SingletonDictionary<Microsoft.Azure.Cosmos.Container> _containers;

    private readonly string? _databaseName;

    public CosmosContainerUtil(ICosmosClientUtil cosmosClientUtil, ICosmosContainerSetupUtil cosmosContainerSetupUtil, IConfiguration config, ILogger<CosmosContainerUtil> logger)
    {
        _logger = logger;

        var ensureContainerOnFirstUse = config.GetValueStrict<bool>("Azure:Cosmos:EnsureContainerOnFirstUse");
        _databaseName = config.GetValueStrict<string>("Azure:Cosmos:DatabaseName");

        _containers = new SingletonDictionary<Microsoft.Azure.Cosmos.Container>(async (containerName, cancellationToken, args) =>
        {
            CosmosClient client = await cosmosClientUtil.Get(cancellationToken).NoSync();

            var databaseName = (string)args[0];

            if (ensureContainerOnFirstUse)
                _ = await cosmosContainerSetupUtil.Ensure(containerName, cancellationToken).NoSync();

            return client.GetContainer(databaseName, containerName);
        });
    }

    public ValueTask<Microsoft.Azure.Cosmos.Container> Get(string containerName, CancellationToken cancellationToken = default)
    {
        return _containers.Get(containerName, cancellationToken, _databaseName!);
    }

    public ValueTask<Microsoft.Azure.Cosmos.Container> Get(string containerName, CosmosClient cosmosClient, string databaseName, CancellationToken cancellationToken = default)
    {
        int hashOfClient = cosmosClient.GetHashCode();

        var containerKey = $"{containerName}-{hashOfClient}";

        return _containers.Get(containerKey, cancellationToken, databaseName, containerName);
    }

    public async ValueTask Delete(string containerName, CancellationToken cancellationToken = default)
    {
        _logger.LogCritical("Deleting container {container}! ...", containerName);

        Microsoft.Azure.Cosmos.Container container = await Get(containerName, cancellationToken).NoSync();
        await container.DeleteContainerAsync(cancellationToken: cancellationToken).NoSync();

        await _containers.Remove(containerName, cancellationToken).NoSync();

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
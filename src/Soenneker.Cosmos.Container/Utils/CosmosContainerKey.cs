namespace Soenneker.Cosmos.Container.Utils;

public readonly record struct CosmosContainerKey(string Endpoint, string DatabaseName, string ContainerName);
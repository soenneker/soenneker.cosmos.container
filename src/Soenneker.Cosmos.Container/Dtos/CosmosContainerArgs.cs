namespace Soenneker.Cosmos.Container.Utils;

public readonly record struct CosmosContainerArgs(string Endpoint, string AccountKey, string DatabaseName, string ContainerName);
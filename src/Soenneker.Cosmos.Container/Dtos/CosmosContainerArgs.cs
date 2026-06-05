namespace Soenneker.Cosmos.Container.Utils;

/// <summary>
/// Represents the cosmos container args record structure.
/// </summary>
/// <param name="Endpoint">The endpoint.</param>
/// <param name="AccountKey">The account key.</param>
/// <param name="DatabaseName">The database name.</param>
/// <param name="ContainerName">The container name.</param>
public readonly record struct CosmosContainerArgs(string Endpoint, string AccountKey, string DatabaseName, string ContainerName);
namespace Soenneker.Cosmos.Container.Dtos;

/// <summary>
/// Represents the cosmos container key record structure.
/// </summary>
/// <param name="Endpoint">The endpoint.</param>
/// <param name="DatabaseName">The database name.</param>
/// <param name="ContainerName">The container name.</param>
public readonly record struct CosmosContainerKey(string Endpoint, string DatabaseName, string ContainerName);
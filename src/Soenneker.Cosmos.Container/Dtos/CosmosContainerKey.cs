namespace Soenneker.Cosmos.Container.Dtos;

public readonly record struct CosmosContainerKey(string Endpoint, string DatabaseName, string ContainerName)
{
    public string? AccountKeyHash { get; init; }
}

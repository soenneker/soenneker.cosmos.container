[![](https://img.shields.io/nuget/v/Soenneker.Cosmos.Container.svg?style=for-the-badge)](https://www.nuget.org/packages/Soenneker.Cosmos.Container/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.cosmos.container/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.cosmos.container/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/Soenneker.Cosmos.Container.svg?style=for-the-badge)](https://www.nuget.org/packages/Soenneker.Cosmos.Container/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.cosmos.container/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.cosmos.container/actions/workflows/codeql.yml)

# Soenneker.Cosmos.Container

Provides cached Azure Cosmos DB container handles plus list and delete operations.

## Install

```bash
dotnet add package Soenneker.Cosmos.Container
```

## Configuration

```json
{
  "Azure": {
    "Cosmos": {
      "Endpoint": "https://your-account.documents.azure.com:443/",
      "AccountKey": "your-account-key",
      "DatabaseName": "application",
      "EnsureContainerOnFirstUse": true
    }
  }
}
```

`EnsureContainerOnFirstUse` defaults to `true`. On the first `Get` for a container, the setup utility attempts to create it with partition key path `/partitionKey` before returning the SDK handle. Set it to `false` when infrastructure provisioning owns container creation or your containers use another partition key path.

## Registration

```csharp
using Soenneker.Cosmos.Container.Registrars;

services.AddCosmosContainerUtilAsSingleton();
```

The registration includes the client, database, and container-setup dependencies and intentionally uses a singleton lifetime.

## Get a container

```csharp
using Microsoft.Azure.Cosmos;
using Soenneker.Cosmos.Container.Abstract;

public sealed class OrderStore(ICosmosContainerUtil containers)
{
    public ValueTask<Container> Get(CancellationToken cancellationToken)
    {
        return containers.Get("orders", cancellationToken);
    }
}
```

Use the explicit overload for another account or database:

```csharp
Container container = await containers.Get(
    endpoint,
    accountKey,
    databaseName,
    containerName,
    cancellationToken);
```

Handles are cached by endpoint, a SHA-256 identity of the account key, database name, and container name. A rotated account key therefore receives a handle backed by the correct client without putting the raw key into the cache key.

## List and delete

```csharp
IReadOnlyList<ContainerProperties> existing = await containers.GetAll(cancellationToken);
await containers.Delete("obsolete", cancellationToken);
```

`GetAll` follows every Cosmos feed page and buffers all `ContainerProperties` in a list. `DeleteAll` lists the database and then deletes each container sequentially.

## Operational notes

- `Delete` and `DeleteAll` permanently remove Cosmos resources and their data. Scope credentials narrowly and do not expose these methods directly to untrusted input.
- `Delete` obtains the cached handle before issuing the service deletion, then evicts that handle after success. A failed service deletion remains cached and the exception propagates.
- Do not dispose returned `Container` handles. The singleton utilities own the underlying clients and caches.
- Account keys are credentials. Store them in a secret provider and keep them out of logs and source control.

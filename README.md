[![](https://img.shields.io/nuget/v/Soenneker.Cosmos.Container.svg?style=for-the-badge)](https://www.nuget.org/packages/Soenneker.Cosmos.Container/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.cosmos.container/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.cosmos.container/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/Soenneker.Cosmos.Container.svg?style=for-the-badge)](https://www.nuget.org/packages/Soenneker.Cosmos.Container/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.cosmos.container/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.cosmos.container/actions/workflows/codeql.yml)

# Soenneker.Cosmos.Container

A utility library for storing Azure Cosmos containers Singleton IoC.

## Install

```bash
dotnet add package Soenneker.Cosmos.Container
```

## Quick start

```csharp
using Soenneker.Cosmos.Container.Registrars;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
var result = services.AddCosmosContainerUtilAsSingleton();
```

Registers Cosmos Container Util with a singleton lifetime.

## What you get

- `ICosmosContainerUtil` — A utility library for storing Azure Cosmos containers Singleton IoC.
- `CosmosContainerUtilRegistrar` — A utility library for storing Azure Cosmos containers.

## API at a glance

| API | What it does | Result / important behavior |
| --- | --- | --- |
| `ICosmosContainerUtil.Get(containerName, cancellationToken)` | Implements double check locking mechanism. | A task whose result is the requested microsoft.Azure.Cosmos.Container. |
| `ICosmosContainerUtil.Delete(containerName, cancellationToken)` | Removes the entry associated with the specified key. | Completes when the requested deletion has finished. |
| `ICosmosContainerUtil.Delete(endpoint, accountKey, databaseName, containerName, cancellationToken)` | Removes the entry associated with the specified key. | Completes when the requested deletion has finished. |
| `CosmosContainerUtilRegistrar.AddCosmosContainerUtilAsSingleton(services)` | Registers Cosmos Container Util with a singleton lifetime. | The same service collection, so additional registrations can be chained. |

## Practical notes

- Cancellation stops pending work; it does not undo work that has already completed.
- Calls that return a cached or singleton value reuse the same instance until the owning service is disposed.
- Dispose instances you own when their scope ends so held resources can be released.

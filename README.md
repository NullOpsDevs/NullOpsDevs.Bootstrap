# 🚀 NullOpsDevs.Bootstrap

[![NuGet](https://img.shields.io/nuget/v/NullOpsDevs.Bootstrap)](https://www.nuget.org/packages/NullOpsDevs.Bootstrap/)
[![NuGet](https://img.shields.io/nuget/vpre/NullOpsDevs.Bootstrap
)](https://www.nuget.org/packages/NullOpsDevs.Bootstrap/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)](https://dotnet.microsoft.com/)
[![.NET](https://img.shields.io/badge/.NET-9.0-purple.svg)](https://dotnet.microsoft.com/)
[![.NET](https://img.shields.io/badge/Documentation-blue)](https://bootstrap.nullops.systems/)

A lightweight and elegant .NET library for orchestrating application bootstrap operations. Define startup actions as a pipeline, track their execution status, and expose bootstrap progress via HTTP middleware.

## ✨ Features

- **Pipeline-Based Bootstrap**: Define and execute startup actions in a structured, sequential pipeline
- **Status Monitoring**: Built-in HTTP middleware to expose bootstrap status and progress
- **Flexible Error Handling**: Choose between service shutdown or continuation on bootstrap failure
- **Dependency Injection**: First-class support for .NET dependency injection
- **Multi-Framework**: Supports .NET 8.0 and .NET 9.0
- **Lightweight**: Minimal dependencies and overhead

## 📦 Installation

Install via .NET CLI:

```bash
dotnet add package NullOpsDevs.Bootstrap
```

Or add to your `.csproj` file:

```xml
<ItemGroup>
  <PackageReference Include="NullOpsDevs.Bootstrap" Version="1.0.0" />
</ItemGroup>
```

## 🚀 Quick Start

### 1. Create a Bootstrap Action

Define a bootstrap action by implementing `IBootstrapPipelineAction`:

```csharp
using NullOpsDevs.Bootstrap.Base;

internal class DatabaseMigrationAction : IBootstrapPipelineAction
{
    public string Name => "Database Migration";

    public async Task<bool> Invoke(CancellationToken cancellationToken)
    {
        // Your bootstrap logic here
        // For example: run database migrations, warm up caches, etc.
        await RunMigrationsAsync(cancellationToken);

        // Return true if successful, false otherwise
        return true;
    }
}
```

### 2. Register the Pipeline

Add your bootstrap actions to the pipeline in `Program.cs`:

```csharp
using NullOpsDevs.Bootstrap;
using NullOpsDevs.Bootstrap.Enums;

var builder = WebApplication.CreateBuilder(args);

// Register the bootstrap pipeline
builder.Services.AddBootstrapPipeline(x =>
{
    x.Use<DatabaseMigrationAction>();
    x.Use<CacheWarmupAction>();
    x.Use<HealthCheckAction>();
});

// Register bootstrap services
builder.Services.UseBootstrap(ErrorBootstrapBehavior.Continue);

var app = builder.Build();

// Trigger bootstrap execution
app.Services.GetRequiredService<IBootstrapService>();

// Add bootstrap status middleware
app.UseBootstrapMiddleware();

app.Run();
```

### 3. Monitor Bootstrap Status

The middleware automatically exposes bootstrap status. When a request is made during bootstrap, the middleware returns:

```json
{
  "state": "InProgress",
  "currentAction": "Database Migration",
  "message": "Service is starting up..."
}
```

## 📖 Usage

### Error Handling Strategies

Choose how your application handles bootstrap failures:

#### ExitOnError (Recommended for Production)

The service will shut down on bootstrap failure. Ideal for containerized environments with automatic restart:

```csharp
builder.Services.UseBootstrap(ErrorBootstrapBehavior.ExitOnError);
```

Use this when:
- Running in Docker, Kubernetes, or systemd with restart policies
- Bootstrap failure means the service cannot function properly
- You want to ensure a clean restart on failure

#### Continue (Recommended for Development)

The service continues running but the middleware always returns an error:

```csharp
builder.Services.UseBootstrap(ErrorBootstrapBehavior.Continue);
```

Use this when:
- Developing and debugging bootstrap actions
- You need the service to stay running for diagnostics
- Bootstrap failure is non-critical

### Bootstrap States

The bootstrap service tracks the following states:

| State        | Description                                  |
|--------------|----------------------------------------------|
| `InProgress` | Bootstrap actions are currently executing    |
| `Successful` | All bootstrap actions completed successfully |
| `Error`      | One or more bootstrap actions failed         |

### Dependency Injection in Actions

Bootstrap actions support constructor injection:

```csharp
internal class CacheWarmupAction : IBootstrapPipelineAction
{
    private readonly ILogger<CacheWarmupAction> _logger;
    private readonly ICacheService _cacheService;

    public string Name => "Cache Warmup";

    public CacheWarmupAction(
        ILogger<CacheWarmupAction> logger,
        ICacheService cacheService)
    {
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task<bool> Invoke(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting cache warmup...");
        await _cacheService.WarmupAsync(cancellationToken);
        _logger.LogInformation("Cache warmup completed");
        return true;
    }
}
```

## 🎯 Use Cases

- **Database Migrations**: Run EF Core migrations before accepting requests
- **Cache Warmup**: Pre-populate caches with frequently accessed data
- **Configuration Validation**: Verify configuration and external dependencies
- **Health Checks**: Validate connectivity to external services (databases, APIs, message queues)
- **Data Seeding**: Initialize default data or reference tables
- **Service Registration**: Register with service discovery systems

## 🏗️ Architecture

The library consists of several key components:

- **IBootstrapPipelineAction**: Interface for defining bootstrap actions
- **IBootstrapService**: Service that manages pipeline execution
- **BootstrapMiddleware**: HTTP middleware for status reporting
- **DefaultBootstrapPipeline**: Default pipeline implementation
- **DefaultStartupPipelineBuilder**: Fluent builder for pipeline configuration

## 📚 Examples

For a complete working example, check out the [example project](https://github.com/NullOpsDevs/NullOpsDevs.Bootstrap/tree/main/src/NullOpsDevs.Bootstrap.Example).

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🔗 Links

- [Documentation](https://bootstrap.nullops.systems/)
- [NuGet Package](https://www.nuget.org/packages/NullOpsDevs.Bootstrap/)
- [GitHub Repository](https://github.com/NullOpsDevs/NullOpsDevs.Bootstrap)
- [Report Issues](https://github.com/NullOpsDevs/NullOpsDevs.Bootstrap/issues)

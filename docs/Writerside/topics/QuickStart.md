# Quickstart

### 1. Install NuGet package

<tabs>
    <tab id="dotnet-CLI-install" title="dotnet CLI">
        <code-block lang="shell">
            dotnet add NullOpsDevs.Bootstrap
        </code-block>
    </tab>
    <tab id="csproj-install" title=".csproj">
        <code-block lang="xml">
            &lt;ItemGroup&gt;
              &lt;PackageReference Include=&quot;NullOpsDevs.Bootstrap&quot; Version=&quot;&lt;...&gt;&quot; /&gt;
            &lt;/ItemGroup&gt;
        </code-block>
    </tab>
</tabs>

### 2. Create your first bootstrap action

```C#
internal class ExampleBootstrapAction : IBootstrapPipelineAction
{
    // Name of the action.
    // Will be returned by the middleware to the frontend.
    public string Name => "Example action";
    
    // Actual task logic.
    public async Task<bool> Invoke(CancellationToken cancellationToken)
    {
        // Artificial delay.
        await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
        
        // Return true if your action succeeded or false otherwise.
        return true;
    }
}
```

### 3. Register your pipeline

```C#
builder.Services.AddBootstrapPipeline(x =>
{
    x.Use<ExampleBootstrapAction>();
});
```

### 4. Register required services and a middleware

```C#
builder.Services.UseBootstrap();

// ... and after WebApplicationBuilder.Build()

app.UseBootstrapMiddleware();
```

This method also accepts `ErrorBootstrapBehavior` as an argument. It specifies how service should handle erroneous state.

### 4.1. Select which `ErrorBootstrapBehavior` do you want

| Value         | Behavior                                                                                                                   |
|---------------|----------------------------------------------------------------------------------------------------------------------------|
| `ExitOnError` | The service will shutdown on bootstrap error. Can be used for automatic restart (when running under docker, systemd, etc.) |
| `Continue`    | Service will continue working, middleware will always return an error.                                                     |


### 5. Full source code

Here's what our `Program.cs` looks like:

```C#
using Void.Libs.Bootstrap;
using Void.Libs.Bootstrap.Base;
using Void.Libs.Bootstrap.Enums;
using Void.Libs.Bootstrap.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddBootstrapPipeline(x =>
{
    x.Use<ExampleBootstrapAction>();
});

builder.Services.UseBootstrap(ErrorBootstrapBehavior.Continue);

var app = builder.Build();

app.Services.GetRequiredService<IBootstrapService>();

app.UseBootstrapMiddleware();
app.MapControllers();
app.Run();

internal class ExampleBootstrapAction : IBootstrapPipelineAction
{
    public string Name => "Example action";
    
    public async Task<bool> Invoke(CancellationToken cancellationToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
        return true;
    }
}
```

For a complete example, please see the [example source code](https://github.com/0x25CBFC4F/Void.Libs.Bootstrap/tree/main/src/Void.Libs.Bootstrap.Example).

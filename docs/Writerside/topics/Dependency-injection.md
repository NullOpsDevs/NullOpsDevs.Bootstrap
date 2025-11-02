# Dependency injection

Bootstrap actions have full dependency injection support. \
Just inject any services you want right into the constructor.

```C#
internal class ExampleBootstrapAction(IExampleService service) : IBootstrapPipelineAction
{
    public string Name => "Example action";
    
    public async Task<bool> Invoke(CancellationToken cancellationToken)
    {
        return await service.InitAsync();
    }
}
```
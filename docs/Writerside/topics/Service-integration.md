# Wait for bootstrap in your code

## How to integrate with `IBootstrapService`

If you have any [Hosted Services](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-8.0) in your code - you'd probably want to wait until your service is in the consistent state.

Here's how you can do this:

1. Inject `IBootstrapService` into your service via DI.
2. Call any of the methods to wait for bootstrap to happen.

There are 4 methods available in total, depending on your coding style:

| Method                                             | Description                                                                                             |
|----------------------------------------------------|---------------------------------------------------------------------------------------------------------|
| `Task<BootstrapState> WaitForBootstrap()`          | Wait and return current server's state.                                                                 |
| `Task<bool> IsBootstrapSuccessful()`               | Wait and return `true` if bootstrap was successful.                                                     |
| `Task AssertBootstrapSuccessful()`                 | Wait and throw an `BootstrapException` if bootstrap wasn't successful.                                  |
| `vvoid Subscribe(Action<BootstrapState> callback)` | Subscribe for state changes. Callback will only be called *once*!                                       |

## Full example

```C#
public class DependentService(ILogger<DependentService> logger, IBootstrapService bootstrapService) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Waiting until bootstrap actions are done..");

        // Inline assert with exception:
        // await bootstrapService.AssertBootstrapSuccessful(stoppingToken);
        
        // Exception-less check:
        if (!await bootstrapService.IsBootstrapSuccessful(stoppingToken))
        {
            logger.LogError("Bootstrap has failed.");
            return;
        }
        
        // Explicit check:
        // if(await bootstrapService.WaitForBootstrap(stoppingToken) == BootstrapState.Successful)
        
        logger.LogInformation("Bootstrap actions are done.");
    }
}
```

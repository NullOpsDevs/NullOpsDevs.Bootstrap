using NullOpsDevs.Bootstrap.Services;

namespace NullOpsDevs.Bootstrap.Example;

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

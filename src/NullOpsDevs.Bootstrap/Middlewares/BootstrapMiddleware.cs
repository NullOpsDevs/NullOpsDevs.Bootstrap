using System.Net;
using NullOpsDevs.Bootstrap.Enums;
using NullOpsDevs.Bootstrap.Models;
using NullOpsDevs.Bootstrap.Services;

namespace NullOpsDevs.Bootstrap.Middlewares;

/// <summary>
/// Startup middleware.
/// </summary>
public class BootstrapMiddleware : IMiddleware
{
    private readonly IBootstrapService bootstrapService;
    private BootstrapState currentState = BootstrapState.InProgress;
    
    /// <summary>
    /// Startup middleware.
    /// </summary>
    public BootstrapMiddleware(IBootstrapService bootstrapService)
    {
        this.bootstrapService = bootstrapService;
        this.bootstrapService.Subscribe(FinalStateHasChanged);
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // If current state is successful - passthrough all requests.
        if (currentState == BootstrapState.Successful)
        {
            await next(context);
            return;
        }
        
        // Otherwise - return 'maintenance' error.
        context.Response.StatusCode = (int) HttpStatusCode.ServiceUnavailable;

        var response = new BootstrapResponse { State = currentState };

        if (currentState == BootstrapState.InProgress)
        {
            context.Response.Headers.Append("Retry-After", "1");
            response.CurrentTask = bootstrapService.CurrentTaskName;
        }

        context.Response.Headers.Append("X-Maintenance", "true");
        context.Response.Headers.Append("Cache-Control", "no-cache");
        
        await context.Response.WriteAsJsonAsync(response);
    }

    private void FinalStateHasChanged(BootstrapState state)
    {
        currentState = state;
    }
}
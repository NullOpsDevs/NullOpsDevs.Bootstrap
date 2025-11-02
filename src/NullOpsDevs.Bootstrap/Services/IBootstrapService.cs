using JetBrains.Annotations;
using NullOpsDevs.Bootstrap.Enums;
using NullOpsDevs.Bootstrap.Exceptions;

namespace NullOpsDevs.Bootstrap.Services;

[PublicAPI]
public interface IBootstrapService : IHostedService
{
    /// <summary>
    /// Current state of the bootstrapper.
    /// </summary>
    BootstrapState State { get; }

    /// <summary>
    /// Current task name.
    /// </summary>
    string? CurrentTaskName { get; }
    
    /// <summary>
    /// Subscribes to the startup state change. <br/>
    /// ⚠️ Warning: State <see cref="BootstrapState.InProgress"/> will not be broadcasted.
    /// </summary>
    void Subscribe(Action<BootstrapState> callback);

    /// <summary>
    /// Waits for bootstrap and returns current state.
    /// </summary>
    /// <returns></returns>
    Task<BootstrapState> WaitForBootstrap(CancellationToken cancellationToken = default);

    /// <summary>
    /// Waits for bootstrap and returns <see langword="true" /> if it was successful.
    /// </summary>
    Task<bool> IsBootstrapSuccessful(CancellationToken cancellationToken = default);

    /// <summary>
    /// Waits for bootstrap and throws an <see cref="BootstrapException"/> if it wasn't successful.
    /// </summary>
    /// <exception cref="BootstrapException">Bootstrap operation was not successful.</exception>
    Task AssertBootstrapSuccessful(CancellationToken cancellationToken = default);
}
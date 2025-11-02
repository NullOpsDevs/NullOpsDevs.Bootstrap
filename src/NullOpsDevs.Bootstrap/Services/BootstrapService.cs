using JetBrains.Annotations;
using NullOpsDevs.Bootstrap.Base;
using NullOpsDevs.Bootstrap.Enums;
using NullOpsDevs.Bootstrap.Exceptions;

namespace NullOpsDevs.Bootstrap.Services;

/// <summary>
/// Startup service. Actually does the bootstrapping actions.
/// </summary>
[PublicAPI]
public class BootstrapService(
    AbstractBootstrapPipeline pipeline,
    ILoggerFactory loggerFactory,
    IServiceProvider serviceProvider,
    IHostApplicationLifetime hostLifetime,
    ErrorBootstrapBehavior errorBootstrapBehavior) : BackgroundService, IBootstrapService
{
    #region Dependencies

    private readonly ILogger _logger = loggerFactory.CreateLogger<BootstrapService>();
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    #endregion

    #region State management

    private readonly object _stateLock = new();
    private BootstrapState _state = BootstrapState.InProgress;

    /// <inheritdoc />
    public BootstrapState State
    {
        get
        {
            lock (_stateLock)
            {
                return _state;
            }
        }
    }
    private void SetStateFinal(BootstrapState newState)
    {
        if (_cancellationTokenSource.IsCancellationRequested)
            throw new InvalidOperationException("Final state was already set.");
        
        lock (_stateLock)
        {
            _state = newState;
        }
        
        _cancellationTokenSource.Cancel();
    }

    public string? CurrentTaskName { get; private set; }

    #endregion

    #region Public methods / subscriptions

    /// <inheritdoc />
    public void Subscribe(Action<BootstrapState> callback)
    {
        if (_cancellationTokenSource.IsCancellationRequested)
        {
            // Special case: someone subscribed after we're already done. Just call the callback immediately.
            callback(State);
            return;
        }
        
        _cancellationTokenSource.Token.Register(_ => callback(State), null);
    }

    /// <inheritdoc />
    public async Task<BootstrapState> WaitForBootstrap(CancellationToken cancellationToken = default)
    {
        // Moving current thread to ThreadPool thread to not stop default host from pausing
        // and prevent other thread and await-related shenanigans.
        await Task.Yield();
        
        var manualResetEvent = new ManualResetEventSlim(false);
        Subscribe(_ => manualResetEvent.Set());
        
        manualResetEvent.Wait(cancellationToken);
        return State;
    }

    /// <inheritdoc />
    public async Task<bool> IsBootstrapSuccessful(CancellationToken cancellationToken = default)
    {
        return await WaitForBootstrap(cancellationToken) == BootstrapState.Successful;
    }

    /// <inheritdoc />
    public async Task AssertBootstrapSuccessful(CancellationToken cancellationToken = default)
    {
        if (await WaitForBootstrap(cancellationToken) != BootstrapState.Successful)
            throw new BootstrapException();
    }

    #endregion

    #region Task execution

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var actions = pipeline.GeneratePipeline().ToArray();

        // Executing startup actions
        await ExecuteStartupActions(stoppingToken, actions);

        var finalState = State;
        
        // Checking final status and exiting the service, if required by user.
        if (finalState == BootstrapState.Error && errorBootstrapBehavior == ErrorBootstrapBehavior.ExitOnError)
        { 
            _logger.LogError("Service will be stopped.");
            hostLifetime.StopApplication();
            return;
        }
        
        if(finalState == BootstrapState.Successful)
            _logger.LogInformation("The bootstrap has been successful.");
    }

    private async Task ExecuteStartupActions(CancellationToken stoppingToken, Type[] actions)
    {
        _logger.LogInformation("Starting up services. Total actions: {TotalActions}.", actions.Length);
        
        for (var actionIndex = 0; actionIndex < actions.Length; actionIndex++)
        {
            // Handling server shutdown
            if (stoppingToken.IsCancellationRequested)
            {
                SetBootstrapCancelled();
                return;
            }
            
            // Creating action instance using DI
            var actionType = actions[actionIndex];
            var instance = CreateActionInstance(actionType, actionIndex, actions);

            if (instance == null)
                return;

            // Running an action
            _logger.LogInformation("[{ActionIndex}/{ActionsTotal}] Running '{Action}'...", actionIndex + 1, actions.Length, instance.Name);
            CurrentTaskName = instance.Name;

            try
            {
                var runResult = await instance.Invoke(stoppingToken);
                
                // If action itself says startup wasn't successful - also setting final state and short-circuiting.
                if (runResult) continue;
                
                _logger.LogError("[{ActionIndex}/{ActionsTotal}] '{Action}' signaled an error during bootstrap.", actionIndex + 1, actions.Length, instance.Name);
                SetStateFinal(BootstrapState.Error);
                return;
            }
            // Handling server shutdown if it happened inside IBootstrapPipelineAction
            catch (OperationCanceledException)
            {
                SetBootstrapCancelled();
                return;
            }
            // Handling everything else.
            catch (Exception exception)
            {
                _logger.LogError(exception, "Unhandled exception caught in startup action '{Action}'", instance.Name);
                SetStateFinal(BootstrapState.Error);
                return;
            }
        }
        
        SetStateFinal(BootstrapState.Successful);
        CurrentTaskName = null;
    }

    private IBootstrapPipelineAction? CreateActionInstance(Type actionType, int actionIndex, Type[] actions)
    {
        IBootstrapPipelineAction? instance;

        try
        {
            instance = (IBootstrapPipelineAction) ActivatorUtilities.CreateInstance(serviceProvider, actionType);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "[{ActionIndex}/{ActionsTotal}] Failed to instanciate an action of type {ActionType}", actionIndex + 1, actions.Length, actionType.FullName);
            SetStateFinal(BootstrapState.Error);
            return null;
        }

        return instance;
    }

    private void SetBootstrapCancelled()
    {
        _logger.LogError("Server bootstrap cancelled due to server shutting down.");
        SetStateFinal(BootstrapState.Error);
    }

    #endregion
}

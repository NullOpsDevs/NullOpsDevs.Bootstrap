using NullOpsDevs.Bootstrap.Services;

namespace NullOpsDevs.Bootstrap.Enums;

/// <summary>
/// Startup behavior on error.
/// </summary>
public enum ErrorBootstrapBehavior
{
    /// <summary>
    /// Exit service. <br/>
    /// Can be used for automatic restart (when running under docker, systemd, etc.) <br/>
    /// </summary>
    ExitOnError,
    
    /// <summary>
    /// Service will continue working. <br/>
    /// - Middleware will serve an error. <br/>
    /// - <see cref="BootstrapService"/> will return invalid status.
    /// </summary>
    Continue
}

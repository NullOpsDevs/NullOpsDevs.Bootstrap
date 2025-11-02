namespace NullOpsDevs.Bootstrap.Enums;

/// <summary>
/// Startup state.
/// </summary>
public enum BootstrapState : short
{
    /// <summary>
    /// Bootstrap is in progress.
    /// </summary>
    InProgress,
    
    /// <summary>
    /// Bootstrap was successful.
    /// </summary>
    Successful,
    
    /// <summary>
    /// Bootstrap was unsuccessful.
    /// </summary>
    Error
}
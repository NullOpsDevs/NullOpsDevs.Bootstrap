namespace NullOpsDevs.Bootstrap.Base;

/// <summary>
/// Startup pipeline action - does something to set up your server. <br/>
/// </summary>
public interface IBootstrapPipelineAction
{
    /// <summary>
    /// Name of the action. Will be returned by middleware as-is. <br/>
    /// Meaning you can use "Migrating database" or "db_migrate"
    /// and handle things accordingly on the frontend.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Actual logic of your action.
    /// </summary>
    /// <returns><see cref="Task"/></returns>
    Task<bool> Invoke(CancellationToken cancellationToken);
}

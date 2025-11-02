namespace NullOpsDevs.Bootstrap.Base;

/// <summary>
/// Class that generates a pipeline based on your needs. <br/>
/// You can create your own <see cref="AbstractBootstrapPipeline"/> to bootstrap
/// your application differently based on the environment.
/// </summary>
public abstract class AbstractBootstrapPipeline
{
    /// <summary>
    /// Should return an <see cref="IEnumerable{T}"/> of <see cref="Type"/>s actions that
    /// should be executed to bootstrap your application.
    /// </summary>
    public abstract IEnumerable<Type> GeneratePipeline();
}

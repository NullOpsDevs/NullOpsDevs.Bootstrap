using NullOpsDevs.Bootstrap.Base;

namespace NullOpsDevs.Bootstrap.Pipeline;

/// <summary>
/// Startup pipeline builder.
/// </summary>
public sealed class DefaultStartupPipelineBuilder
{
    private readonly List<Type> _startupActions = [];
    
    /// <summary>
    /// Adds 
    /// </summary>
    /// <typeparam name="TAction"></typeparam>
    /// <returns></returns>
    public DefaultStartupPipelineBuilder Use<TAction>()
        where TAction : IBootstrapPipelineAction
    {
        var type = typeof(TAction);
        
        if (_startupActions.Any(x => x == type))
            throw new InvalidOperationException($"Startup action of type '{type.FullName}' is already added to the pipeline.");
        
        _startupActions.Add(type);
        return this;
    }

    internal DefaultBootstrapPipeline Build() => new(_startupActions);
}

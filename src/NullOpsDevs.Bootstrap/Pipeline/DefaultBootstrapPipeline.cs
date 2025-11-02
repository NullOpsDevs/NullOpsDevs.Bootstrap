using NullOpsDevs.Bootstrap.Base;

namespace NullOpsDevs.Bootstrap.Pipeline;

/// <summary>
/// Default (and simple) startup pipeline. <br/>
/// Will return all <see cref="IBootstrapPipelineAction"/> that you give it. 
/// </summary>
public sealed class DefaultBootstrapPipeline : AbstractBootstrapPipeline
{
    private readonly IEnumerable<Type> _actionTypes;

    internal DefaultBootstrapPipeline(IEnumerable<Type> actionTypes)
    {
        _actionTypes = actionTypes;
    }

    /// <inheritdoc />
    public override IEnumerable<Type> GeneratePipeline() => _actionTypes;

    /// <summary>
    /// Creates a simple pipeline from <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <param name="actions">Actions that need to be executed.</param>
    public static DefaultBootstrapPipeline Create(IEnumerable<Type> actions) => new(actions);
}
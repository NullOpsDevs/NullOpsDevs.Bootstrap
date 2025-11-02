using NullOpsDevs.Bootstrap.Services;

namespace NullOpsDevs.Bootstrap.Exceptions;

/// <summary>
/// Exception which will be thrown upon failed bootstrap in <see cref="BootstrapService"/>.<see cref="BootstrapService.IsBootstrapSuccessful"/>
/// </summary>
public class BootstrapException() : InvalidOperationException("Bootstrap has failed.");

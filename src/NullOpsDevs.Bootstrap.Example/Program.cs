using NullOpsDevs.Bootstrap;
using NullOpsDevs.Bootstrap.Base;
using NullOpsDevs.Bootstrap.Enums;
using NullOpsDevs.Bootstrap.Example;
using NullOpsDevs.Bootstrap.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddBootstrapPipeline(x =>
{
    x.Use<ExampleBootstrapAction>();
});

builder.Services.AddSingleton<DependentService>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<DependentService>());

builder.Services.UseBootstrap(ErrorBootstrapBehavior.Continue);

var app = builder.Build();

app.Services.GetRequiredService<IBootstrapService>();

app.UseBootstrapMiddleware();
app.MapControllers();
app.Run();

internal class ExampleBootstrapAction : IBootstrapPipelineAction
{
    public string Name => "Example action";
    
    public async Task<bool> Invoke(CancellationToken cancellationToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
        return true;
    }
}

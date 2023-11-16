using CliWrap;
using CodeSlackers.HostedConsole;
using CodeSlackers.HostedConsole.Logging;
using CodeSlackers.SolutionBuilder.State;
using Microsoft.Extensions.Logging;


namespace CodeSlackers.SolutionBuilder.Flows;

public class SolutionFlow(
        IFlowIoService flowIoService,
        IStateService<SolutionBuilderState> stateService,
        ILogger<SolutionFlow> logger)
    : IFlow
{
    public string FlowName => SolutionBuilderFlows.SolutionFlow;
    public string NextFlow { get; set; } = SolutionBuilderFlows.Quit;

    public async Task Run()
    {
        var state = stateService.GetState();
        var diretory = $"{state.WorkingDirectory}/{state.SolutionName}";
        logger.LogEvent($"Creating solution {state.SolutionName}", state);

        Directory.CreateDirectory($"{state.WorkingDirectory}/{state.SolutionName}");
        await Cli.Wrap("dotnet").WithArguments($"new sln -n {state.SolutionName}")
            .WithWorkingDirectory($"{state.WorkingDirectory}/{state.SolutionName}").ExecuteAsync();
        await Cli.Wrap("dotnet").WithArguments("new gitignore").WithWorkingDirectory(diretory).ExecuteAsync();
        await Cli.Wrap("git").WithArguments("init").WithWorkingDirectory(diretory).ExecuteAsync();
        Directory.CreateDirectory($"{state.WorkingDirectory}/{state.SolutionName}/src");
        NextFlow = SolutionBuilderFlows.ProjectFlow;
    }
}
using CliWrap;
using CodeSlackers.HostedConsole;
using CodeSlackers.SolutionBuilder.State;

namespace CodeSlackers.SolutionBuilder.Flows;

public class FinalizeFlow(IStateService<SolutionBuilderState> stateService, IFlowIoService flowIoService ) : IFlow
{
    public string FlowName => SolutionBuilderFlows.FinalizeFlow;
    public string NextFlow { get; set; }

    public async Task Run()
    {
        var state = stateService.GetState();
        if (state.Projects.Any(p => p.ProjectType == ProjectType.Library))
        {
            foreach (var project in state.Projects.Where(p=>p.ProjectType == ProjectType.Library))
            {
                var menu = MenuManager.YesNo(() =>
                {
                    AddReferences(project.Name, state);
                }, 
                (() => { }), $"Do you want to all other projects to reference {project.Name} ?"
             );
                await menu.ShowAsync();
            }
        }
        flowIoService.WriteLine("Your solution is ready!");
        NextFlow = SolutionBuilderFlows.Quit;
    }

    private void AddReferences(string name,  SolutionBuilderState state)
    {
        var rootDirectory = $"{state.WorkingDirectory}/{state.SolutionName}/src/";
        var projectReferencePath = $"{state.WorkingDirectory}/{state.SolutionName}/src/{name}/{name}.csproj";
        var projectsToAddReference = state.Projects.Where(p =>  p.Name != name);
        foreach (var project in projectsToAddReference)
        {
            var task = Cli.Wrap("dotnet")
                .WithArguments(
                    $"add {rootDirectory}{project.Name}/{project.Name}.csproj reference {projectReferencePath}")
                .ExecuteAsync();

            task.Task.Wait();
        }
    }

}
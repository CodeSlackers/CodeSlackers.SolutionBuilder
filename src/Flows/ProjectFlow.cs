using CodeSlackers.HostedConsole;
using CodeSlackers.HostedConsole.Logging;
using CodeSlackers.SolutionBuilder.State;
using Microsoft.Extensions.Logging;


namespace CodeSlackers.SolutionBuilder.Flows;

public class ProjectFlow(IFlowIoService flowIoService, IStateService<SolutionBuilderState> stateService, ILogger<ProjectFlow> logger) : IFlow
{
    

    public string FlowName => SolutionBuilderFlows.ProjectFlow;
    public string NextFlow { get; set; } = SolutionBuilderFlows.FinalizeFlow;
    public async Task Run()
    {
        var projectType = ProjectType.NeverMind;
        var state = stateService.GetState();

        projectType = await SelectProjectType();

        var projectName = GetProjectName(state, projectType);
        
        await AddProject(state, projectName, projectType);
        logger.LogEvent($"Added project {projectName} of {projectType} ", state);

        // Set Next Flow
        var menu = MenuManager.YesNo(
            () => { NextFlow = SolutionBuilderFlows.ProjectFlow; }, () =>
            {
                logger.LogEvent("Solution Built", state);
                NextFlow = SolutionBuilderFlows.FinalizeFlow;
            },
            "Do you wish to add another project?");
        await menu.ShowAsync();
    }

    private static async Task<ProjectType> SelectProjectType()
    {
        ProjectType projectType = ProjectType.NeverMind;
        var menuQuestions = MenuManager.SelectEnum<ProjectType>(
            (selectedProjectType, menu) =>
            {
                projectType = selectedProjectType;
                menu.CloseMenu();
            }, "Please Select a Project Type");

        await menuQuestions.ShowAsync();
        return projectType;
    }


    private async Task AddProject(SolutionBuilderState state, string projectName, ProjectType projectType)
    {
        var template = GetProjectTemplate(projectType);
        if (template == string.Empty)
        {
            return;
        }
        var resultOne = await CliWrap.Cli.Wrap("dotnet").WithArguments($"new {template} -o src/{projectName}").WithWorkingDirectory($"{state.WorkingDirectory}/{state.SolutionName}").ExecuteAsync();
        logger.LogInformation(resultOne.ExitCode == 0 ? $"{projectName} created" : "Project failed to create!");
        var resultTwo = await CliWrap.Cli.Wrap("dotnet").WithArguments(@$"dotnet sln add src/{projectName}").WithWorkingDirectory($"{state.WorkingDirectory}/{state.SolutionName}").ExecuteAsync();
        logger.LogInformation(resultTwo.ExitCode == 0 ? $"{projectName} added to solution" : "Failed to add to solution");
    }

    private string GetProjectTemplate(ProjectType projectType) =>
        projectType switch
        {
            ProjectType.WebApi => "webapi -minimal false",
            ProjectType.Worker => "worker",
            ProjectType.Blazor => "blazorwasm",
            ProjectType.BlazorServer => "blazorserver",
            ProjectType.Library => "classlib",
            ProjectType.NeverMind => "",
        };


    private string GetProjectName(SolutionBuilderState state, ProjectType projectType)
    {
        var projectName = string.Empty;
        flowIoService.WriteLine("Please provide project name");
        bool validName = false;
        while (validName == false)
        {
            projectName = flowIoService.ReadLine();
            if (string.IsNullOrEmpty(projectName) || projectName.ToLower() == "quit")
            {
                flowIoService.WriteLine("Please enter the name of your solution or projectType quit to exit");
                projectName = flowIoService.ReadLine();
            }

            if (projectName.ToLower() == "quit")
            {
                return projectName;
            }

            if (state.Projects.Any(p=> p.Name == projectName))
            {
                flowIoService.WriteLine("Project name already exists, please enter a new name");
            }
            else
            {
                state.Projects.Add(new Project{Name = projectName, ProjectType = projectType});

                validName = true;
            }
        }

        return projectName;
    }
}
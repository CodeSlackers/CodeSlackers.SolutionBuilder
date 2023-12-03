using CodeSlackers.HostedConsole;
using CodeSlackers.SolutionBuilder.Flows;
using CodeSlackers.SolutionBuilder.State;
using Microsoft.Extensions.DependencyInjection;

namespace CodeSlackers.SolutionBuilder;

class Program
{
    private static string _workingDirectory = string.Empty;
    private static string _solutionName = string.Empty;
    static async Task Main(string[] args)
    {
        if (args.Length > 0 && !string.IsNullOrEmpty(args[0]))
        {
            _solutionName = args[0];
            _workingDirectory = Directory.GetCurrentDirectory();
        }
        else
        {
            if (PromptUserForSolution()) return;
        }

        var screen = ConsoleScreenAppBuilder.CreateConfigureConsoleScreenApplication(
            SolutionBuilderFlows.SolutionBuilderScreen,
            (_, collection) =>
            {
                collection.AddSingleton<IFlowIoService, FlowIoService>();
                collection.AddSingleton<IStateService<SolutionBuilderState>>(new StateService(_solutionName, _workingDirectory));
                collection.AddTransient<IFlow, SolutionFlow>();
                collection.AddTransient<IFlow, ProjectFlow>();
                collection.AddTransient<IFlow, FinalizeFlow>();
                collection.AddSingleton<IConsoleScreen, SolutionBuilderScreen>();
            });

        await screen.Show();
    }

    private static bool PromptUserForSolution()
    {
        while (string.IsNullOrEmpty(_workingDirectory))
        {
            Console.WriteLine("Please provide a working directory, or type 'Quit' to exit");
            _workingDirectory = Console.ReadLine();
            if (_workingDirectory?.ToLower() == "quit")
            {
                return true;
            }
        }

        while (string.IsNullOrEmpty(_solutionName))
        {
            Console.WriteLine("Please provide a solution name, or type 'Quit' to exit");
            _solutionName = Console.ReadLine();
            if (_solutionName?.ToLower() == "quit")
            {
                return true;
            }
        }

        return false;
    }
}
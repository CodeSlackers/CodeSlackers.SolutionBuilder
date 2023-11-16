using CodeSlackers.HostedConsole;
using CodeSlackers.SolutionBuilder.State;

namespace CodeSlackers.SolutionBuilder;

public class StateService : IStateService<SolutionBuilderState>
{
    private readonly SolutionBuilderState _state = new();

    public StateService(string solutionName, string workingDirectory)
    {
        _state.SolutionName = solutionName;
        _state.WorkingDirectory = workingDirectory;
    }

    

    public SolutionBuilderState GetState()
    {
        return _state;
    }
}
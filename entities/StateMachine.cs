using Godot;
using System.Collections.Generic;

public class StateMachine : Node
{
    private readonly Dictionary<string, State> stateByName = new Dictionary<string, State>();
    private State currentState;

    public override void _Ready()
    {
        var stateNodes = GetChildren();
        var initialStates = new List<State>(stateNodes.Count);
        for (int nodeIndex = 0; nodeIndex < stateNodes.Count; nodeIndex++)
        {
            if (stateNodes[nodeIndex] is State)
            {
                var state = (State)stateNodes[nodeIndex];
                if (currentState == null)
                {
                    currentState = state;
                }
                stateByName.Add(state.Name, state);
            }
        }
    }

    public override void _Process(float delta)
    {
        if (currentState == null)
        {
            return;
        }
        currentState.TargetState = null;
        currentState.Process(delta);
        checkForTargetState();
    }

    public override void _PhysicsProcess(float delta)
    {
        if (currentState == null)
        {
            return;
        }
        currentState.TargetState = null;
        currentState.PhysicsProcess(delta);
        checkForTargetState();
    }

    private void checkForTargetState()
    {
        if (currentState.TargetState != null)
        {
            if (currentState.TargetState.Equals(currentState.Name))
            {
                GD.PushWarning("Cannot switch to state '" + currentState.TargetState + "' from the same state.");
                return;
            }
            if (!stateByName.ContainsKey(currentState.TargetState))
            {
                GD.PushWarning("Cannot switch to state '" + currentState.TargetState + "' because it does not exist.");
                return;
            }
            currentState.Exit();
            currentState = stateByName[currentState.TargetState];
            currentState.Enter();
        }
    }
}

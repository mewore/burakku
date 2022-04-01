using Godot;

public abstract class State : Node
{
    public string TargetState;

    public virtual void Enter()
    {
    }

    public virtual void Exit()
    {
    }

    public virtual void PhysicsProcess(float delta)
    {
    }

    public virtual void Process(float delta)
    {
    }
}

using Godot;

public class PlayerActive : PlayerState
{
    public override void PhysicsProcess(float delta)
    {
        player.Move(delta, true);
    }

    public override void UnhandledInput(InputEvent @event)
    {
        CheckEnterDoor(@event);
    }
}

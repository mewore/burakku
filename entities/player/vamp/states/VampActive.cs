using Godot;

public class VampActive : VampState
{
    public override void PhysicsProcess(float delta)
    {
        vamp.Move(delta, true);
        float hp = vamp.CheckForDamage(delta);
        if (hp < 0f)
        {
            targetState = DYING;
        }
    }

    public override void Process(float delta)
    {
        vamp.RenderFire(delta, false);
        vamp.UpdateFireSound(delta, false);
    }

    public override void UnhandledInput(InputEvent @event)
    {
        CheckEnterDoor(@event);
    }
}

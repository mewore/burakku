using Godot;
using System;

public class VampActive : VampState
{
    public override void PhysicsProcess(float delta)
    {
        vamp.Move(delta, true);
        float hp = vamp.CheckForDamage(delta);
        if (hp < 0f)
        {
            TargetState = DYING;
        }
    }

    public override void Process(float delta)
    {
        vamp.RenderFire();
    }
}

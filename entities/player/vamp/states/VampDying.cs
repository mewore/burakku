using Godot;

public class VampDying : VampState
{
    [Signal]
    delegate void Finished();

    private const float MAX_MOTION_SQUARED = 20f;
    private const float ENERGY_DECREASE_PER_SECOND = .5f;

    private AnimationPlayer animationPlayer;
    private bool finished = false;

    [Export]
    private bool activelyBurning = true;

    public override void _Ready()
    {
        base._Ready();
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    public override void Enter()
    {
        animationPlayer.Play("die");
    }

    public override void PhysicsProcess(float delta)
    {
        vamp.Move(delta, false);
        if (!finished && !animationPlayer.IsPlaying() && vamp.IsOnFloor() && vamp.Motion.LengthSquared() <= MAX_MOTION_SQUARED)
        {
            animationPlayer.Play("stop_burning");
        }
    }

    public override void Process(float delta)
    {
        vamp.ClearFire();
        vamp.RenderFire(delta, activelyBurning);
        vamp.UpdateFireSound(delta, activelyBurning);
    }

    public void Finish()
    {
        finished = true;
        EmitSignal(nameof(Finished));
    }
}

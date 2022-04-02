using Godot;

public class VampDying : VampState
{
    [Signal]
    delegate void Finished();

    private const float MAX_MOTION_SQUARED = 20f;

    private AnimationPlayer animationPlayer;
    private bool finished = false;

    public override void _Ready()
    {
        base._Ready();
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    public override void Enter()
    {
        GD.Print("Playing 'die'");
        animationPlayer.Play("die");
    }

    public override void PhysicsProcess(float delta)
    {
        vamp.Move(delta, false);
        if (!finished && !animationPlayer.IsPlaying() && vamp.IsOnFloor() && vamp.Motion.LengthSquared() <= MAX_MOTION_SQUARED)
        {
            GD.Print("Playing 'stop_burning'");
            animationPlayer.Play("stop_burning");
        }
    }

    public override void Process(float delta)
    {
        vamp.ClearFire();
    }

    public void Finish()
    {
        finished = true;
        EmitSignal(nameof(Finished));
    }
}

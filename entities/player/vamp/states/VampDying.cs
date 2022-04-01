using Godot;

public class VampDying : VampState
{
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
        animationPlayer.Play("die");
    }

    public override void PhysicsProcess(float delta)
    {
        vamp.Move(delta, false);
        if (!animationPlayer.IsPlaying() && vamp.Motion.LengthSquared() <= MAX_MOTION_SQUARED)
        {
            animationPlayer.Play("stop_burning");
        }
    }

    public override void Process(float delta)
    {
        vamp.ClearFire();
    }

    public void Finish()
    {
        GetTree().ReloadCurrentScene();
    }
}

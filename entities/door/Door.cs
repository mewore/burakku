using Godot;

public class Door : StaticBody2D, Operable
{
    private AnimationPlayer animationPlayer;

    [Export]
    private bool opened;

    private bool activated;

    public override void _Ready()
    {
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    public override void _PhysicsProcess(float delta)
    {
        if (!animationPlayer.IsPlaying() && activated != opened)
        {
            animationPlayer.Play(activated ? "open" : "close");
        }
    }

    public void Activate()
    {
        activated = true;
    }

    public void Deactivate()
    {
        activated = false;
    }
}

using Godot;

public class Overlay : CanvasLayer
{
    [Signal]
    delegate void FadeInDone();

    [Signal]
    delegate void FadeOutDone();

    private AnimationPlayer animationPlayer;

    public override void _Ready()
    {
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        GetNode<CanvasItem>("Polygon2D").Visible = true;
    }

    public void FadeIn()
    {
        animationPlayer.Queue("fade_in");
    }

    public void FadeOut()
    {
        animationPlayer.Queue("fade_out");
    }

    public void FadeOutReverse()
    {
        animationPlayer.Queue("fade_out_reverse");
    }

    public void _on_AnimationPlayer_animation_finished(string animationName)
    {
        if (animationName.Equals("fade_in"))
        {
            EmitSignal(nameof(FadeInDone));
        }
        else if (animationName.StartsWith("fade_out"))
        {
            EmitSignal(nameof(FadeOutDone));
        }
    }
}

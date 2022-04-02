using Godot;

public class Level : Node2D
{
    private Overlay overlay;

    public override void _Ready()
    {
        overlay = GetNode<Overlay>("Overlay");
        overlay.FadeIn();
    }

    public void _on_Overlay_FadeOutDone()
    {
        GetTree().ReloadCurrentScene();
    }

    public void _on_Vamp_Died()
    {
        overlay.FadeOut();
    }

    public void _on_WinDoor_Won()
    {
        overlay.FadeOut();
    }
}

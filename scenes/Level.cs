using Godot;

public class Level : Node2D
{
    private string targetScene;
    private Overlay overlay;

    public override void _Ready()
    {
        overlay = GetNode<Overlay>("Overlay");
        overlay.FadeIn();
    }

    public void _on_Overlay_FadeOutDone()
    {
        GetTree().ChangeScene("scenes/" + targetScene + ".tscn");
    }

    public void _on_Vamp_Died()
    {
        targetScene = GetTree().CurrentScene.Name;
        overlay.FadeOutReverse();
    }

    public void _on_WinDoor_Won()
    {
        targetScene = GetTree().CurrentScene.Name.EndsWith("1") ? "Level2" : "Level1";
        overlay.FadeOut();
    }
}

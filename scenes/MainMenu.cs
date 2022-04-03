using Godot;

public class MainMenu : Node2D
{
    private Button playButton;
    private string targetScene;
    private Overlay overlay;

    public override void _Ready()
    {
        playButton = GetNode<Button>("Container/VBoxContainer/PlayButton");
        overlay = GetNode<Overlay>("Overlay");
        overlay.FadeIn();
    }

    public void _on_PlayButton_pressed()
    {
        playButton.Disabled = true;
        overlay.FadeOut();
    }

    public void _on_Overlay_FadeOutDone()
    {
        GetTree().ChangeScene(Global.CurrentLevelPath);
    }
}

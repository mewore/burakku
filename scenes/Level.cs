using Godot;

public class Level : Node2D
{
    private Overlay overlay;
    private int currentLevel;

    public override void _Ready()
    {
        overlay = GetNode<Overlay>("Overlay");
        overlay.FadeIn();
        GetTree().Paused = true;
        currentLevel = Global.CurrentLevel;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("debug_clear_level"))
        {
            WinLevel();
        }
    }

    public void _on_Overlay_FadeInDone()
    {
        GetTree().Paused = false;
    }

    public void _on_Overlay_FadeOutDone()
    {
        GetTree().ChangeScene(Global.CurrentLevelPath);
    }

    public void _on_Vamp_Died()
    {
        GetTree().Paused = true;
        overlay.FadeOutReverse();
    }

    public void _on_WinDoor_Won()
    {
        WinLevel();
    }

    private void WinLevel()
    {
        GetTree().Paused = true;
        Global.WinLevel(currentLevel);
        overlay.FadeOut();
    }
}

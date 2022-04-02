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
        GetTree().ChangeScene(GetScenePath(targetScene));
    }

    public void _on_Vamp_Died()
    {
        targetScene = GetTree().CurrentScene.Name;
        overlay.FadeOutReverse();
    }

    public void _on_WinDoor_Won()
    {
        targetScene = "Level" + (GetTree().CurrentScene.Name.Replace("Level", "").ToInt() + 1);
        if (!new File().FileExists(GetScenePath(targetScene)))
        {
            targetScene = "Level1";
        }
        overlay.FadeOut();
    }

    private static string GetScenePath(string sceneName)
    {
        return "scenes/" + sceneName + ".tscn";
    }
}

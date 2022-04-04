using Godot;

public class GlobalSound : Node
{
    private AudioStreamPlayer clearLevel;

    public override void _Ready()
    {
        clearLevel = GetNode<AudioStreamPlayer>("ClearLevel");
    }

    public void PlayClearLevel()
    {
        clearLevel.Play();
    }

    public static GlobalSound GetInstance(Node node)
    {
        return node.GetNode<GlobalSound>("/root/GlobalSound");
    }
}

using Godot;

public class StaticBlacklight : Node2D, Operable
{
    private BlacklightSource blacklightSource;

    public override void _Ready()
    {
        blacklightSource = GetNode<BlacklightSource>("BlacklightSource");
    }

    public void TurnOn()
    {
        blacklightSource.TurnOn();
    }
    public void TurnOff()
    {
        blacklightSource.TurnOff();
    }

    public void Activate() => TurnOff();
    public void Deactivate() => TurnOn();
}

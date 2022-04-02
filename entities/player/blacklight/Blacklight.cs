public class Blacklight : Player
{
    private BlacklightSource blacklightSource;

    public override void _Ready()
    {
        base._Ready();
        blacklightSource = GetNode<BlacklightSource>("BlacklightSource");
    }

    public override bool TryToEnter()
    {
        if (base.TryToEnter())
        {
            blacklightSource.TurnOff();
            return true;
        }
        return false;
    }

    public override bool TryToLeave()
    {
        if (base.TryToLeave())
        {
            blacklightSource.TurnOn();
            return true;
        }
        return false;
    }
}

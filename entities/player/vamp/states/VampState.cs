public class VampState : PlayerState
{
    protected const string DYING = "Dying";

    protected Vamp vamp;

    public override void _Ready()
    {
        base._Ready();
        vamp = GetOwner<Vamp>();
    }
}

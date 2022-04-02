using Godot;

public class PlayerState : State
{
    protected const string ACTIVE = "Active";
    protected const string INSIDE_DOOR = "InsideDoor";

    protected Player player;

    public override void _Ready()
    {
        player = GetOwner<Player>();
    }

    protected void CheckEnterDoor(InputEvent @event)
    {
        if (@event.IsActionPressed(player.EnterInput) && player.TryToEnter())
        {
            targetState = INSIDE_DOOR;
        }
    }
}

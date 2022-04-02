using Godot;

public class PlayerInsideDoor : PlayerState
{
    [Export]
    private bool canExit;

    public override void Enter()
    {
        canExit = false;
    }

    public override void Process(float delta)
    {
        if (canExit && Input.IsActionPressed(player.LeaveInput))
        {
            player.TryToLeave();
        }
    }

    public void PlayerExited()
    {
        targetState = PlayerState.ACTIVE;
    }
}

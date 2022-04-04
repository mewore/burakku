using Godot;

public class VampInsideDoor : VampState
{
    [Export]
    private bool canExit;

    public override void Enter()
    {
        canExit = false;
        vamp.ClearFire();
    }

    public override void Process(float delta)
    {
        if (canExit && Input.IsActionPressed(player.LeaveInput))
        {
            player.TryToLeave();
        }
        vamp.UpdateFireSound(delta, false);
    }

    public void PlayerExited()
    {
        targetState = PlayerState.ACTIVE;
    }
}

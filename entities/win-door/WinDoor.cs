using Godot;

public class WinDoor : Node2D
{
    private const float FADE_SPEED = 4f;

    private int playersInside = 0;
    private int playersInRange = 0;

    public bool HasWon { get => playersInside >= 2; }

    private CanvasItem enterTip;
    private CanvasItem leaveTip;

    public override void _Ready()
    {
        enterTip = GetNode<CanvasItem>("Tips/EnterTip");
        UpdateOpacity(enterTip, 0f, 1f);
        leaveTip = GetNode<CanvasItem>("Tips/LeaveTip");
        UpdateOpacity(leaveTip, 0f, 1f);
    }

    public override void _Process(float delta)
    {
        float maxOpacityChange = FADE_SPEED * delta;
        UpdateOpacity(enterTip, (playersInRange - playersInside) > 0 ? 1f : 0f, maxOpacityChange);
        UpdateOpacity(leaveTip, (playersInside > 0 && playersInside < 2) ? 1f : 0f, maxOpacityChange);
    }

    private static void UpdateOpacity(CanvasItem item, float target, float maxChange)
    {
        float difference = Mathf.Abs(target - item.Modulate.a);
        if (difference > 0f)
        {
            item.Modulate = new Color(item.Modulate, difference <= maxChange
                ? target
                : item.Modulate.a + Mathf.Sign(target - item.Modulate.a) * maxChange);
        }
    }

    public void PlayerEntered()
    {
        ++playersInside;
    }

    public void PlayerLeft()
    {
        --playersInside;
    }

    public void PlayerInRange()
    {
        ++playersInRange;
    }

    public void PlayerOutOfRange()
    {
        --playersInRange;
    }
}

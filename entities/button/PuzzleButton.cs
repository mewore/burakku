using Godot;

public class PuzzleButton : Node2D
{
    private int pressers = 0;

    [Export]
    private NodePath target = null;

    private Operable targetNode;

    private Node2D innerButton;

    private float notPressedY;
    private float pressedY;

    public override void _Ready()
    {
        targetNode = target == null ? null : GetNode<Operable>(target);
        innerButton = GetNode<Node2D>("Inner");
        notPressedY = innerButton.Position.y;
        pressedY = notPressedY * .5f;
    }

    public void _on_PressArea_body_entered(Node body)
    {
        if ((++pressers) == 1 && targetNode != null)
        {
            targetNode.Activate();
            innerButton.Position = new Vector2(innerButton.Position.x, pressedY);
        }
    }

    public void _on_PressArea_body_exited(Node body)
    {
        if ((--pressers) == 0 && targetNode != null)
        {
            targetNode.Deactivate();
            innerButton.Position = new Vector2(innerButton.Position.x, notPressedY);
        }
    }
}

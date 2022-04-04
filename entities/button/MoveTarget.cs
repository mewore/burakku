using Godot;

public class MoveTarget : Position2D, Operable
{
    [Export]
    private Curve movementCurve;

    [Export]
    private float travelTime = 1f;

    [Export]
    private NodePath body = null;

    private StaticBody2D bodyNode;
    private Vector2 sourcePosition;
    private Vector2 targetPosition;

    private float closenessToTarget = 0f;

    private bool activated;

    public override void _Ready()
    {
        bodyNode = body == null ? GetParent<StaticBody2D>() : GetNode<StaticBody2D>(body);
        sourcePosition = bodyNode.GlobalPosition;
        targetPosition = GlobalPosition;
    }

    public override void _PhysicsProcess(float delta)
    {
        float targetCloseness = activated ? 1f : 0f;
        float closenessDifference = Mathf.Abs(closenessToTarget - targetCloseness);
        if (closenessDifference > 0f)
        {
            float maxClosenessChange = delta / travelTime;
            closenessToTarget = closenessDifference < maxClosenessChange
                ? targetCloseness
                : closenessToTarget + maxClosenessChange * Mathf.Sign(targetCloseness - closenessToTarget);
            var actualCloseness = movementCurve.InterpolateBaked(closenessToTarget);
            Vector2 newBodyPosition = sourcePosition * (1f - actualCloseness) + targetPosition * actualCloseness;
            Vector2 relativeToBody = bodyNode.ToLocal(newBodyPosition);
            bodyNode.ConstantLinearVelocity = relativeToBody / delta;
            bodyNode.Position += relativeToBody;
        }
    }

    public void Activate() => activated = true;

    public void Deactivate() => activated = false;
}

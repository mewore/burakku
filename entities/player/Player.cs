using Godot;

public class Player : KinematicBody2D
{
    private const float ACCELERATION = 800f;
    private const float AIR_ACCELERATION = 400f;
    private const float MAX_SPEED = 100f;

    private const float MAX_FALL_SPEED = 300f;
    private const float GRAVITY = 500f;

    private Vector2 motion = new Vector2();

    [Export]
    private string walkLeftInputName = null;

    [Export]
    private string walkRightInputName = null;

    public override void _PhysicsProcess(float delta)
    {
        bool isOnFloor = IsOnFloor();
        float maxAcceleration = (isOnFloor ? ACCELERATION : AIR_ACCELERATION) * delta;

        float targetMotionX = (Input.GetActionStrength(walkRightInputName) - Input.GetActionStrength(walkLeftInputName)) * MAX_SPEED;
        motion.x = Mathf.Abs(targetMotionX - motion.x) <= maxAcceleration
            ? targetMotionX
            : motion.x + (targetMotionX > motion.x ? maxAcceleration : -maxAcceleration);

        motion.y = Mathf.Min(isOnFloor ? motion.y : (motion.y + GRAVITY * delta), MAX_FALL_SPEED);

        motion = MoveAndSlide(motion, Vector2.Up);
    }
}

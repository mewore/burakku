using Godot;

public class Player : KinematicBody2D
{
    private const float ACCELERATION = 1600f;
    private const float AIR_ACCELERATION = 400f;
    private const float MAX_SPEED = 120f;

    private const float MAX_FALL_SPEED = 300f;
    private const float GRAVITY = 600f;

    private float jumpSpeed;
    private const float JUMP_SPEED_RETENTION = .5f;
    private const float JUMP_GRACE_PERIOD = .1f;
    private float lastWantedToJumpAt = -JUMP_GRACE_PERIOD * 2f;
    private float lastOnFloorAt = -JUMP_GRACE_PERIOD * 2f;
    private bool isJumping = false;

    private Vector2 motion = new Vector2();

    private float now = 0f;

    [Export]
    private string walkLeftInputName = null;

    [Export]
    private string walkRightInputName = null;

    public override void _Ready()
    {
        float jumpHeight = -GetNode<Node2D>("MaxJumpHeight").Position.y;
        jumpSpeed = Mathf.Sqrt(GRAVITY * jumpHeight * 2f);
    }

    public override void _PhysicsProcess(float delta)
    {
        bool isOnFloor = IsOnFloor();
        float maxAcceleration = (isOnFloor ? ACCELERATION : AIR_ACCELERATION) * delta;

        float targetMotionX = (Input.GetActionStrength(walkRightInputName) - Input.GetActionStrength(walkLeftInputName)) * MAX_SPEED;
        motion.x = Mathf.Abs(targetMotionX - motion.x) <= maxAcceleration
            ? targetMotionX
            : motion.x + (targetMotionX > motion.x ? maxAcceleration : -maxAcceleration);

        motion.y = Mathf.Min(isOnFloor ? motion.y : (motion.y + GRAVITY * delta), MAX_FALL_SPEED);

        if (Input.IsActionJustPressed("jump"))
        {
            lastWantedToJumpAt = now;
        }
        if (isOnFloor)
        {
            lastOnFloorAt = now;
        }

        if (!isJumping && Mathf.Max(now - lastWantedToJumpAt, now - lastOnFloorAt) <= JUMP_GRACE_PERIOD)
        {
            lastOnFloorAt = lastWantedToJumpAt = now - JUMP_GRACE_PERIOD;
            motion.y = -jumpSpeed;
            isJumping = true;
        }
        else if (isJumping && Input.IsActionJustReleased("jump"))
        {
            motion = new Vector2(motion.x, motion.y * JUMP_SPEED_RETENTION);
            isJumping = false;
        }
        isJumping = isJumping && motion.y < 0f;

        motion = MoveAndSlide(motion, Vector2.Up);

        now += delta;
    }
}

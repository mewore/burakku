using Godot;

public class Player : KinematicBody2D
{
    private const float ACCELERATION = 1600f;
    private const float AIR_ACCELERATION = 400f;
    private const float INACTIVE_ACCELERATION = 100f;
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
    public Vector2 Motion { get => motion; }

    private AnimationPlayer animationPlayer;

    private float now = 0f;

    private const float DOOR_ENTER_GRACE_PERIOD = .05f;
    private WinDoor door;
    private CollisionShape2D doorDetectionShape;
    private bool insideDoor = false;
    public bool InsideDoor { get => insideDoor; }
    private Vector2 enterDoorSourcePosition = Vector2.Zero;
    private Vector2 enterDoorTargetPosition = Vector2.Zero;

    [Export]
    private float closenessToDoor { get => 0f; set => Position = enterDoorSourcePosition.LinearInterpolate(enterDoorTargetPosition, value); }

    [Export]
    private string inputSuffix = "";

    [Export]
    private string walkLeftInput = "move_left";
    private string WalkLeftInput { get => walkLeftInput + "_" + inputSuffix; }

    [Export]
    private string walkRightInput = "move_right";
    private string WalkRightInput { get => walkRightInput + "_" + inputSuffix; }

    [Export]
    private string jumpInput = "jump";
    private string JumpInput { get => jumpInput + "_" + inputSuffix; }

    [Export]
    private string enterInput = "enter";
    public string EnterInput { get => enterInput + "_" + inputSuffix; }

    [Export]
    private string leaveInput = "leave";
    public string LeaveInput { get => leaveInput + "_" + inputSuffix; }

    public override void _Ready()
    {
        float jumpHeight = -GetNode<Node2D>("MaxJumpHeight").Position.y;
        jumpSpeed = Mathf.Sqrt(GRAVITY * jumpHeight * 2f);
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        doorDetectionShape = GetNode<CollisionShape2D>("DoorDetector/CollisionShape2D");
    }

    public override void _PhysicsProcess(float delta)
    {
        now += delta;
    }

    public void Move(float delta, bool canControl)
    {
        bool isOnFloor = IsOnFloor();
        float maxAcceleration = (canControl ? (isOnFloor ? ACCELERATION : AIR_ACCELERATION) : INACTIVE_ACCELERATION) * delta;

        float targetMotionX = canControl
            ? (Input.GetActionStrength(WalkRightInput) - Input.GetActionStrength(WalkLeftInput)) * MAX_SPEED
            : 0;
        motion.x = Mathf.Abs(targetMotionX - motion.x) <= maxAcceleration
            ? targetMotionX
            : motion.x + (targetMotionX > motion.x ? maxAcceleration : -maxAcceleration);

        motion.y = Mathf.Min(isOnFloor ? motion.y : (motion.y + GRAVITY * delta), MAX_FALL_SPEED);

        if (Input.IsActionJustPressed(JumpInput))
        {
            lastWantedToJumpAt = now;
        }
        if (isOnFloor)
        {
            lastOnFloorAt = now;
        }

        if (canControl)
        {
            if (!isJumping && Mathf.Max(now - lastWantedToJumpAt, now - lastOnFloorAt) <= JUMP_GRACE_PERIOD)
            {
                lastOnFloorAt = lastWantedToJumpAt = now - JUMP_GRACE_PERIOD;
                motion.y = -jumpSpeed;
                isJumping = true;
            }
            else if (isJumping && Input.IsActionJustReleased(JumpInput))
            {
                motion = new Vector2(motion.x, motion.y * JUMP_SPEED_RETENTION);
                isJumping = false;
            }
        }
        isJumping = isJumping && motion.y < 0f;

        motion = MoveAndSlide(motion, Vector2.Up);
        doorDetectionShape.Disabled = !canControl || now - lastOnFloorAt > DOOR_ENTER_GRACE_PERIOD;
    }

    public void _on_DoorDetector_area_entered(Area2D doorArea)
    {
        door = doorArea.GetOwner<WinDoor>();
        door.PlayerInRange();
    }

    public void _on_DoorDetector_area_exited(Area2D doorArea)
    {
        door.PlayerOutOfRange();
        door = null;
    }

    public virtual bool TryToEnter()
    {
        if (door != null)
        {
            door.PlayerEntered();
            insideDoor = true;
            enterDoorSourcePosition = Position;
            enterDoorTargetPosition = door.Position;
            animationPlayer.Play("enter_door");
            return true;
        }
        return false;
    }

    public virtual bool TryToLeave()
    {
        if (door != null && !door.HasWon && !animationPlayer.IsPlaying())
        {
            door.PlayerLeft();
            insideDoor = false;
            animationPlayer.Play("leave_door");
            return true;
        }
        return false;
    }
}

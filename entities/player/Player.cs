using Godot;

public class Player : KinematicBody2D
{
    private const float ACCELERATION = 1600f;
    private const float AIR_ACCELERATION = 400f;
    private const float INACTIVE_ACCELERATION = 100f;
    private const float MAX_SPEED = 120f;

    private const float MAX_FALL_SPEED = 300f;
    private const float GRAVITY = 600f;

    private float now = 0f;

    private float jumpSpeed;
    private const float JUMP_SPEED_RETENTION = .5f;
    private const float JUMP_GRACE_PERIOD = .1f;
    private float lastWantedToJumpAt = -JUMP_GRACE_PERIOD * 2f;
    private float lastOnFloorAt = 0f;
    private bool isJumping = false;

    private Vector2 motion = new Vector2();
    public Vector2 Motion { get => motion; }

    private Sprite sprite;
    private Sprite outlineSprite;
    private AnimationPlayer animationPlayer;
    private AnimationPlayer tipAnimationPlayer;

    private const float DOOR_ENTER_GRACE_PERIOD = .05f;
    private WinDoor door;
    private CollisionShape2D doorDetectionShape;
    private bool insideDoor = false;
    public bool InsideDoor { get => insideDoor; }
    private Vector2 enterDoorSourcePosition = Vector2.Zero;
    private Vector2 enterDoorTargetPosition = Vector2.Zero;

    private AudioStreamPlayer jumpPlayer;

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
        sprite = GetNode<Sprite>("Sprite");
        outlineSprite = GetNode<Sprite>("Sprite/Outline");
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        tipAnimationPlayer = GetNode<AnimationPlayer>("Tip/AnimationPlayer");
        doorDetectionShape = GetNode<CollisionShape2D>("DoorDetector/CollisionShape2D");
        jumpPlayer = GetNode<AudioStreamPlayer>("JumpPlayer");
    }

    public override void _PhysicsProcess(float delta)
    {
        now += delta;
    }

    public override void _Process(float delta)
    {
        outlineSprite.Frame = sprite.Frame;
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

        if (targetMotionX != 0f && tipAnimationPlayer != null && !tipAnimationPlayer.IsPlaying())
        {
            tipAnimationPlayer.Play("fade_out");
            tipAnimationPlayer = null;
        }

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
                jumpPlayer.PitchScale = 1f + (GD.Randf() - .5f) * .5f;
                jumpPlayer.Play();
            }
            else if (isJumping && Input.IsActionJustReleased(JumpInput))
            {
                motion = new Vector2(motion.x, motion.y * JUMP_SPEED_RETENTION);
                isJumping = false;
            }
        }
        isJumping = isJumping && motion.y < 0f;

        motion = MoveAndSlide(motion, Vector2.Up);

        if (canControl && sprite.Visible)
        {
            string targetAnimation = (Mathf.Abs(motion.x) < MAX_SPEED * .2f) ? "idle" : "walk";
            if (now - lastOnFloorAt > JUMP_GRACE_PERIOD)
            {
                targetAnimation = ((motion.y < 0f) ? "jump" : "fall") + (targetAnimation.Equals("idle") ? "" : "_side");
            }
            int motionScale = Mathf.Sign(motion.x);
            if (motionScale != 0 && Mathf.Sign(sprite.Scale.x) != motionScale)
            {
                sprite.Scale = new Vector2(sprite.Scale.x * -1f, sprite.Scale.y);
            }
            if (!targetAnimation.Equals(animationPlayer.CurrentAnimation))
            {
                animationPlayer.Play(targetAnimation);
            }
        }

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

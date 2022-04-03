using Godot;
using System.Collections.Generic;

public class Vamp : Player
{
    [Signal]
    delegate void Died();

    private const float HIT_FIRE_DISTANCE = 12f;
    private const float HP_LOST_PER_HIT_PER_SECOND = .1f;
    private const int DAMAGE_RAYCAST_RESOLUTION_MIN = 5;
    private const int DAMAGE_RAYCAST_RESOLUTION_MAX = 30;
    private const float DAMAGE_RAYCAST_ANGLE_STEP = Mathf.Pi / DAMAGE_RAYCAST_RESOLUTION_MAX;

    private const float DAMAGE_LIGHT_ENERGY_PER_HIT = .3f;
    private const float DAMAGE_LIGHT_ENERGY_CHANGE = 1f;

    private float hp = 1f;

    private List<float> pendingHitAngles = new List<float>();
    private List<float> lastHitAngles = new List<float>();

    [Export]
    private PackedScene hitFireScene = null;

    private List<CPUParticles2D> hitFires = new List<CPUParticles2D>();

    private Vector2 center;
    private float radius;
    private float squaredRadius;

    private BlacklightSource[] blacklightSources;
    private RayCast2D damageRay;

    private LineBar hpBar;

    private Light2D damageLight;
    public Light2D DamageLight { get => damageLight; }

    public override void _Ready()
    {
        base._Ready();
        center = GetNode<Node2D>("Center").Position;
        squaredRadius = (GetNode<Node2D>("Top").Position - center).LengthSquared();
        radius = Mathf.Sqrt(squaredRadius);

        var blacklightSourceNodes = GetTree().GetNodesInGroup("blacklight_source");
        blacklightSources = new BlacklightSource[blacklightSourceNodes.Count];
        for (int index = 0; index < blacklightSourceNodes.Count; index++)
        {
            blacklightSources[index] = (BlacklightSource)blacklightSourceNodes[index];
        }
        damageRay = GetNode<RayCast2D>("DamageRay");
        hpBar = GetNode<LineBar>("HpBar");
        damageLight = GetNode<Light2D>("DamageLight");
    }

    public float CheckForDamage(float delta)
    {
        foreach (BlacklightSource blacklightSource in blacklightSources)
        {
            if (!blacklightSource.TurnedOn)
            {
                continue;
            }
            var squaredDistanceToTarget = blacklightSource.GlobalPosition.DistanceSquaredTo(GlobalPosition);
            if (squaredDistanceToTarget < squaredRadius)
            {
                // Instant death
                hp = -1f;
                continue;
            }
            var angleToCenter = blacklightSource.GlobalPosition.AngleToPoint(GlobalPosition);
            var angleVariation = Mathf.Atan2(radius, Mathf.Sqrt(squaredDistanceToTarget - squaredRadius));
            float minAngle = -angleVariation;
            float maxAngle = angleVariation;
            int steps = Mathf.Clamp((int)((maxAngle - minAngle) / DAMAGE_RAYCAST_ANGLE_STEP), DAMAGE_RAYCAST_RESOLUTION_MIN, DAMAGE_RAYCAST_RESOLUTION_MAX);

            Vector2 centerCastTo = blacklightSource.ToLocal(ToGlobal(center)).Normalized() * blacklightSource.RayLength;
            damageRay.Position = ToLocal(blacklightSource.GlobalPosition);
            float anglePerStep = (maxAngle - minAngle) / Mathf.Max(1, steps - 1);
            float offsetAngle = minAngle;
            for (int rayIndex = 0; rayIndex < steps; rayIndex++, offsetAngle += anglePerStep)
            {
                damageRay.CastTo = centerCastTo.Rotated(offsetAngle);
                damageRay.ForceRaycastUpdate();
                if (damageRay.IsColliding() && damageRay.GetCollider() == this)
                {
                    registerHit(damageRay.GetCollisionPoint());
                }
            }
        }

        hp -= pendingHitAngles.Count * HP_LOST_PER_HIT_PER_SECOND * delta;
        hpBar.Value = hp;
        hpBar.Visible = hp > 0f && hp < 1f;

        lastHitAngles = pendingHitAngles;
        pendingHitAngles = new List<float>();

        return hp;
    }

    public void ClearFire()
    {
        for (int hitIndex = 0; hitIndex < hitFires.Count; hitIndex++)
        {
            hitFires[hitIndex].Emitting = false;
        }
    }

    public void RenderFire()
    {
        for (int hitIndex = 0; hitIndex < lastHitAngles.Count; hitIndex++)
        {
            if (hitIndex >= hitFires.Count)
            {
                CPUParticles2D hitFire = hitFireScene.Instance<CPUParticles2D>();
                hitFires.Add(hitFire);
                AddChild(hitFire);
            }
            hitFires[hitIndex].Position = center + Vector2.Right.Rotated(lastHitAngles[hitIndex]) * HIT_FIRE_DISTANCE;
            hitFires[hitIndex].Rotation = lastHitAngles[hitIndex];
            hitFires[hitIndex].Emitting = true;
        }
        for (int hitIndex = lastHitAngles.Count; hitIndex < hitFires.Count; hitIndex++)
        {
            hitFires[hitIndex].Emitting = false;
        }
    }

    public void registerHit(Vector2 hitPosition)
    {
        pendingHitAngles.Add(ToGlobal(center).AngleToPoint(hitPosition) + Mathf.Pi);
    }

    public void _on_Dying_Finished()
    {
        EmitSignal(nameof(Died));
    }
}

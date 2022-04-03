using Godot;
using System.Collections.Generic;

public class Vamp : Player
{
    [Signal]
    delegate void Died();

    private const float HIT_FIRE_DISTANCE = 8f;
    private const float HP_LOST_PER_HIT_PER_SECOND = .1f;
    private const int DAMAGE_RAYCAST_RESOLUTION_MIN = 5;
    private const int DAMAGE_RAYCAST_RESOLUTION_MAX = 30;
    private const float DAMAGE_RAYCAST_ANGLE_STEP = Mathf.Pi / DAMAGE_RAYCAST_RESOLUTION_MAX;

    private const float DAMAGE_LIGHT_ENERGY_PER_HIT = .3f;
    private const float DAMAGE_LIGHT_ENERGY_CHANGE = 1f;
    private const float DAMAGE_PSEUDO_LIGHT_OPACITY_PER_HIT = .1f;
    private const float DAMAGE_PSEUDO_LIGHT_OPACITY_CHANGE = 1f;
    private const float DAMAGE_PSEUDO_LIGHT_OPACITY_WHEN_BURNING = 1f;

    private float hp = 1f;

    private List<(float, float)> pendingHits = new List<(float, float)>();
    private List<(float, float)> firesToRender = new List<(float, float)>();

    [Export]
    private PackedScene hitFireScene = null;

    private List<CPUParticles2D> hitFires = new List<CPUParticles2D>();

    private Vector2 center;
    private float radius;
    private float squaredRadius;

    private BlacklightSource[] blacklightSources;
    private RayCast2D damageRay;

    private LineBar hpBar;

    private CanvasItem damagePseudoLight;
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
        GetNode<Light2D>("DeathLight").Visible = true;
        damagePseudoLight = GetNode<CanvasItem>("Center/DamagePseudoLight");
    }

    public float CheckForDamage(float delta)
    {
        foreach (BlacklightSource blacklightSource in blacklightSources)
        {
            if (!blacklightSource.TurnedOn)
            {
                continue;
            }
            Vector2 centerPosition = ToGlobal(center);
            var squaredDistanceToTarget = blacklightSource.GlobalPosition.DistanceSquaredTo(centerPosition);
            float anglePerStep = Mathf.Tau / DAMAGE_RAYCAST_RESOLUTION_MAX;
            if (squaredDistanceToTarget < squaredRadius)
            {
                float angle = 0f;
                for (int hitIndex = 0; hitIndex < DAMAGE_RAYCAST_RESOLUTION_MAX; hitIndex++, angle += anglePerStep)
                {
                    RegisterHit(angle, 1f);
                }
                continue;
            }
            var angleToCenter = blacklightSource.GlobalPosition.AngleToPoint(centerPosition);
            var angleVariation = Mathf.Atan2(radius, Mathf.Sqrt(squaredDistanceToTarget - squaredRadius));
            float minAngle = -angleVariation;
            float maxAngle = angleVariation;
            int steps = Mathf.Clamp((int)((maxAngle - minAngle) / DAMAGE_RAYCAST_ANGLE_STEP), DAMAGE_RAYCAST_RESOLUTION_MIN, DAMAGE_RAYCAST_RESOLUTION_MAX);

            Vector2 centerCastTo = blacklightSource.ToLocal(centerPosition).Normalized() * blacklightSource.RayLength;
            float castToLength = centerCastTo.Length();
            damageRay.Position = ToLocal(blacklightSource.GlobalPosition);
            anglePerStep = (maxAngle - minAngle) / Mathf.Max(1, steps - 1);
            float offsetAngle = minAngle;
            for (int rayIndex = 0; rayIndex < steps; rayIndex++, offsetAngle += anglePerStep)
            {
                damageRay.CastTo = centerCastTo.Rotated(offsetAngle);
                damageRay.ForceRaycastUpdate();
                if (damageRay.IsColliding() && damageRay.GetCollider() == this)
                {
                    RegisterHit(damageRay.GetCollisionPoint(), Mathf.Abs(damageRay.GetCollisionNormal().Dot(damageRay.CastTo) / castToLength));
                }
            }
        }

        foreach ((float, float) hit in pendingHits)
        {
            hp -= hit.Item2 * HP_LOST_PER_HIT_PER_SECOND * delta;
        }
        hpBar.Value = hp;
        hpBar.Visible = hp > 0f && hp < 1f;

        firesToRender = pendingHits;
        pendingHits = new List<(float, float)>();

        return hp;
    }

    public void ClearFire()
    {
        firesToRender = new List<(float, float)>();
    }

    public void RenderFire(float delta, bool isBurning)
    {
        for (int hitIndex = 0; hitIndex < firesToRender.Count; hitIndex++)
        {
            if (hitIndex >= hitFires.Count)
            {
                CPUParticles2D hitFire = hitFireScene.Instance<CPUParticles2D>();
                hitFires.Add(hitFire);
                AddChild(hitFire);
            }
            hitFires[hitIndex].Modulate = new Color(hitFires[hitIndex].Modulate, firesToRender[hitIndex].Item2);
            hitFires[hitIndex].Position = center + Vector2.Right.Rotated(firesToRender[hitIndex].Item1) * HIT_FIRE_DISTANCE;
            hitFires[hitIndex].Rotation = firesToRender[hitIndex].Item1;
            hitFires[hitIndex].Emitting = true;
        }
        float maxOpacityChange = DAMAGE_PSEUDO_LIGHT_OPACITY_CHANGE * delta;
        for (int hitIndex = firesToRender.Count; hitIndex < hitFires.Count; hitIndex++)
        {
            hitFires[hitIndex].Emitting = false;
            if (hitFires[hitIndex].Modulate.a > 0f)
            {
                hitFires[hitIndex].Modulate = new Color(hitFires[hitIndex].Modulate, Mathf.Max(0f, hitFires[hitIndex].Modulate.a - maxOpacityChange));
            }
        }
        float targetOpacity = isBurning ? DAMAGE_PSEUDO_LIGHT_OPACITY_WHEN_BURNING : DAMAGE_PSEUDO_LIGHT_OPACITY_PER_HIT * firesToRender.Count;
        float pseudoLightOpacity = damagePseudoLight.Modulate.a;
        float opacityDifference = Mathf.Abs(pseudoLightOpacity - targetOpacity);
        if (opacityDifference > .01f)
        {
            damagePseudoLight.Modulate = opacityDifference <= maxOpacityChange
                ? new Color(damagePseudoLight.Modulate, targetOpacity)
                : new Color(damagePseudoLight.Modulate, pseudoLightOpacity + Mathf.Sign(targetOpacity - pseudoLightOpacity) * maxOpacityChange);
        }
        damagePseudoLight.Visible = damagePseudoLight.Modulate.a > .01f;
    }

    public void RegisterHit(Vector2 hitPosition, float parallel)
    {
        RegisterHit(ToGlobal(center).AngleToPoint(hitPosition) + Mathf.Pi, parallel);
    }

    public void RegisterHit(float angle, float parallel)
    {
        pendingHits.Add((angle, parallel));
    }

    public void _on_Dying_Finished()
    {
        EmitSignal(nameof(Died));
    }
}

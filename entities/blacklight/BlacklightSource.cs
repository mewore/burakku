using Godot;
using System.Collections.Generic;

public class BlacklightSource : Polygon2D
{
    const float ANGLE_OFFSET = .001f;
    const float MIN_ANGLE_DIFFERENCE = ANGLE_OFFSET * .5f;
    const float CIRCLE_RESOLUTION = .05f;

    const float LIGHT_OVERLAP = 3f;

    private static Dictionary<Node2D, Vector2[]> pointsPerNode;
    private static Dictionary<Node2D, (Vector2, float)[]> circlesPerNode;

    private Vector2 rayVector;
    private RayCast2D ray;

    private int counter = 0;

    private const int MINIMUM_RAY_COUNT = 4;
    private float[] mandatoryRayAngles = new float[MINIMUM_RAY_COUNT];

    public override void _Ready()
    {
        float angleStep = Mathf.Tau / MINIMUM_RAY_COUNT;
        float currentAngle = angleStep * .5f;
        for (int rayIndex = 0; rayIndex < MINIMUM_RAY_COUNT; rayIndex++, currentAngle = (currentAngle + angleStep) % Mathf.Tau)
        {
            mandatoryRayAngles[rayIndex] = currentAngle;
        }

        ray = GetNode<RayCast2D>("RayCast2D");
        rayVector = new Vector2(-ray.CastTo.Length(), 0f);
        if (pointsPerNode == null || circlesPerNode == null)
        {
            updateRayTargets(GetTree(), ray.CollisionMask);
        }
    }

    private static void updateRayTargets(SceneTree tree, uint collisionMask)
    {
        pointsPerNode = new Dictionary<Node2D, Vector2[]>();
        circlesPerNode = new Dictionary<Node2D, (Vector2, float)[]>();
        var opaqueBodies = tree.GetNodesInGroup("opaque");
        for (int bodyIndex = 0; bodyIndex < opaqueBodies.Count; bodyIndex++)
        {
            var body = (PhysicsBody2D)opaqueBodies[bodyIndex];
            if ((body.CollisionLayer | collisionMask) == 0)
            {
                continue;
            }
            var children = ((Node)opaqueBodies[bodyIndex]).GetChildren();
            for (int j = 0; j < children.Count; j++)
            {
                var childNode = (Node2D)children[j];
                if (childNode is CollisionShape2D)
                {
                    var shape = ((CollisionShape2D)childNode).Shape;
                    if (shape is RectangleShape2D)
                    {
                        var rect = (RectangleShape2D)shape;
                        pointsPerNode.Add(childNode, new Vector2[]{
                            new Vector2(-rect.Extents.x, -rect.Extents.y),
                            new Vector2(rect.Extents.x, -rect.Extents.y),
                            new Vector2(-rect.Extents.x, rect.Extents.y),
                            new Vector2(rect.Extents.x, rect.Extents.y)
                        });
                    }
                    else if (shape is SegmentShape2D)
                    {
                        var segment = (SegmentShape2D)shape;
                        pointsPerNode.Add(childNode, new Vector2[] { segment.A, segment.B });
                    }
                    else if (shape is CapsuleShape2D)
                    {
                        var capsule = (CapsuleShape2D)shape;
                        var halfHeight = capsule.Height * .5f;
                        if (halfHeight > 0f)
                        {
                            pointsPerNode.Add(childNode, new Vector2[]{
                                new Vector2(-capsule.Radius, -halfHeight),
                                new Vector2(capsule.Radius, -halfHeight),
                                new Vector2(-capsule.Radius, halfHeight),
                                new Vector2(capsule.Radius, halfHeight)
                            });
                        }
                        circlesPerNode.Add(childNode, new (Vector2, float)[]{
                            (new Vector2(0, -halfHeight), capsule.Radius),
                            (new Vector2(0, halfHeight), capsule.Radius)
                        });
                    }
                    else if (shape is CircleShape2D)
                    {
                        circlesPerNode.Add(childNode, new (Vector2, float)[] { (Vector2.Zero, ((CircleShape2D)shape).Radius) });
                    }
                    else
                    {
                        GD.PushWarning("Cannot handle the shape of node " + childNode.GetPath());
                    }
                }
                else if (childNode is CollisionPolygon2D)
                {
                    pointsPerNode.Add(childNode, ((CollisionPolygon2D)childNode).Polygon);
                }
                else
                {
                    GD.PushWarning("Node " + childNode.GetPath() + " is neither a collision shape nor a collision polygon");
                }
            }
        }
    }

    public override void _Process(float delta)
    {
        // Make a list of the necessary raycasting angles
        Vector2 globalPosition = GlobalPosition;
        var angles = new List<float>();
        foreach (var entry in pointsPerNode)
        {
            foreach (var point in entry.Value)
            {
                Vector2 targetGlobalPos = entry.Key.ToGlobal(point);
                float angle = globalPosition.AngleToPoint(targetGlobalPos);
                ray.CastTo = new Vector2(new Vector2(-globalPosition.DistanceTo(targetGlobalPos) + .1f, 0).Rotated(angle));
                ray.ForceRaycastUpdate();
                if (!ray.IsColliding())
                {
                    angles.Add(angle);
                }
            }
        }
        foreach (var entry in circlesPerNode)
        {
            foreach (var circle in entry.Value)
            {
                Vector2 targetGlobalPos = entry.Key.ToGlobal(circle.Item1);
                var squaredDistanceToTarget = globalPosition.DistanceSquaredTo(targetGlobalPos);
                float radius = circle.Item2;
                var squaredRadius = radius * radius;
                if (squaredDistanceToTarget < squaredRadius)
                {
                    Visible = false;
                    return;
                }
                var angleToCenter = globalPosition.AngleToPoint(targetGlobalPos);
                var angleVariation = Mathf.Atan2(radius, Mathf.Sqrt(squaredDistanceToTarget - squaredRadius));
                float minAngle = angleToCenter - angleVariation;
                float maxAngle = angleToCenter + angleVariation;
                for (float angle = minAngle; angle < maxAngle; angle += CIRCLE_RESOLUTION)
                {
                    angles.Add(angle);
                }
                angles.Add(maxAngle);
            }
        }
        Visible = true;
        int initialAngleCount = angles.Count;
        for (int angleIndex = 0; angleIndex < initialAngleCount; angleIndex++)
        {
            angles.Add((angles[angleIndex] + ANGLE_OFFSET + Mathf.Tau) % Mathf.Tau);
            angles[angleIndex] = (angles[angleIndex] - ANGLE_OFFSET + Mathf.Tau * 2f) % Mathf.Tau;
        }
        angles.AddRange(mandatoryRayAngles);
        angles.Sort();
        var finalAngles = new List<float>(angles.Count);
        float lastAngle = angles[0];
        finalAngles.Add(angles[0]);
        for (int angleIndex = 1; angleIndex < angles.Count; angleIndex++)
        {
            float angle = angles[angleIndex];
            if (Mathf.Min(angle - lastAngle, angles[0] + Mathf.Tau - angle) >= MIN_ANGLE_DIFFERENCE)
            {
                lastAngle = angle;
                finalAngles.Add(angle);
            }
        }
        angles = finalAngles;

        // Now that the angles have been calculated, do the raycasting
        var newPolygon = Polygon.Length != angles.Count ? new Vector2[angles.Count] : Polygon;
        for (int angleIndex = 0; angleIndex < angles.Count; angleIndex++)
        {
            ray.CastTo = new Vector2(rayVector.Rotated(angles[angleIndex]));
            ray.ForceRaycastUpdate();
            newPolygon[angleIndex] = ray.IsColliding() ? ToLocal(ray.GetCollisionPoint()) : ray.CastTo;
        }
        Polygon = newPolygon;
    }
}

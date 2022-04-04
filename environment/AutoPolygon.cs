using Godot;
using System;

public class AutoPolygon : Polygon2D
{
    public override void _Ready()
    {
        Node parent = GetParent<Node>();
        if (parent is CollisionPolygon2D)
        {
            Polygon = ((CollisionPolygon2D)parent).Polygon;
        }
        else if (parent is CollisionShape2D)
        {
            var shape = ((CollisionShape2D)parent).Shape;
        }
    }
}

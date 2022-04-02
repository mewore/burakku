using Godot;

public class PolygonLine : Line2D
{
    public override void _Ready()
    {
        var polygon = GetParent<Polygon2D>();
        var points = new Vector2[polygon.Polygon.Length + 1];
        polygon.Polygon.CopyTo(points, 0);
        points[points.Length - 1] = points[0];
        Points = points;
    }
}

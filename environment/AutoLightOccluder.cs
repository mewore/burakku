using Godot;

public class AutoLightOccluder : LightOccluder2D
{
    public override void _Ready()
    {
        OccluderPolygon2D newOccluder = new OccluderPolygon2D();
        newOccluder.Closed = Occluder.Closed;
        newOccluder.CullMode = Occluder.CullMode;
        newOccluder.Polygon = GetParent<Polygon2D>().Polygon;
        Occluder = newOccluder;
    }
}

using Godot;

public class AutoLightOccluder : LightOccluder2D
{
    public override void _Ready()
    {
        Occluder.Polygon = GetParent<Polygon2D>().Polygon;
    }
}

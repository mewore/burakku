using Godot;
using System;

public class VampState : PlayerState
{
    protected const string DYING = "Dying";

    protected Vamp vamp;

    public override void _Ready()
    {
        vamp = GetOwner<Vamp>();
    }
}

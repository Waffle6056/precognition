using Godot;
using System;

public partial class AttackDash : Dash, Attack
{
    [Export]
    public float Damage{get;set;}
    [Export]
    public Area3D Hitbox;
    protected override bool StartAction()
    {
        if (!base.StartAction())
            return false;
        Hitbox.Monitoring = true;
        return true;
    }
    protected override bool EndAction()
    {
        if (!base.EndAction())
            return false;
        Hitbox.Monitoring = false;
        return true;
    }
}

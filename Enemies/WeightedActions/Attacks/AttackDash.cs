using Godot;
using System;

public partial class AttackDash : Dash, Attack
{
    [Export]
    public float Damage{get;set;}
    [Export]
    public Area3D Hitbox;
    protected override void StartAction()
    {
        base.StartAction();
        Hitbox.Monitoring = true;
    }
    protected override void EndAction()
    {
        base.EndAction();
        Hitbox.Monitoring = false;
    }
}

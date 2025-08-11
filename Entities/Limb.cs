using Godot;
using System;

[GlobalClass]
public partial class Limb : Area3D
{
	public Entity Parent;
    [Export]
    public float MassFactor = 1f;
    [Export]
    public bool IsFooting = false;

    public override void _Ready()
    {
        base._Ready();
        Parent = Misc.findParent(this);
    }

}

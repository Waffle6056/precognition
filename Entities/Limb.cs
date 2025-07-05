using Godot;
using System;

[GlobalClass]
public partial class Limb : Area3D
{
	[Export]
	public Entity Parent;
    [Export]
    public float MassFactor = 1f;
    [Export]
    public bool IsFooting = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		if (Parent != null)
		{
			Parent.Limbs.Add(this);
		}
	}
    
}

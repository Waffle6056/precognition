using Godot;
using System;

public partial class TargetFollowCamera : Camera3D
{
	[Export]
	public Node3D Target;
	[Export]
	public Vector3 Offset = new Vector3(0,10,0);
	[Export]
	public float Weight = .5f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Position = Position.Lerp(Target.Position+Offset,Weight);
	}
}

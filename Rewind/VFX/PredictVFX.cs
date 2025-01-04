using Godot;
using System;

public partial class PredictVFX : Node2D
{
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Visible = RewindController.Instance.IsPredicting;	
		Position = DisplayServer.WindowGetSize() / 2;
	}
}

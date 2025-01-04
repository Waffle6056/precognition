using Godot;
using System;
using System.Diagnostics;

public partial class RewindVFX : Node2D
{
	[Export]
	public GpuParticles2D SwirlParticles = null;
	public ParticleProcessMaterial ppm = null;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ppm = (ParticleProcessMaterial) SwirlParticles.ProcessMaterial;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Visible = RewindController.Instance.IsRewinding;
		
		Position = DisplayServer.WindowGetSize() / 2;
	}
}

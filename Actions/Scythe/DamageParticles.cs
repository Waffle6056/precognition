using Godot;
using System;

public partial class DamageParticles : GpuParticles3D
{
	public void Collide(Area3D a)
	{
		Restart();
	}
}

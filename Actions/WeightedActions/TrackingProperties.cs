using Godot;
using System;

[GlobalClass]
public partial class TrackingProperties : Node
{
	[Export]
	public bool SlerpTracking = true;
	[Export]
	public float SlerpWeight = .5f;
    [Export]
    public float MaximumRotationPerSecond = (float)(Math.PI * 2f);
    [Export]
    public float LinearRotationPerSecond = (float)(Math.PI * 0.5f);

}
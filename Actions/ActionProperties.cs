using Godot;
using System;

[GlobalClass]
public partial class ActionProperties : Node
{
	[Export]
	public float ChannelTime = 0;
	[Export]
	public float EndLagTime = 0;
	[Export]
	public Entity Root;
	[Export]
	public bool Interruptable = true;
	
}

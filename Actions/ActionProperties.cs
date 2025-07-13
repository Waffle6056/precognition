using Godot;
using System;

[GlobalClass]
public partial class ActionProperties : Node
{
	[Export]
	public float ChannelTime = 0;
    [Export]
    public float ActingMaximumTime = 600;
    [Export]
	public float EndLagTime = 0; 
    [Export]
    public Option[] FollowUpOptions { get; set; }
    [Export]
    public Action DefaultFollowUp;
	public Entity Root;
	
}

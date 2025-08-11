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
    public Entity Parent;
    [Export]
    public float ActionBufferTime = 1f;
    [Export]
    public TrackingProperties TrackingChange { get; set; }
    public override void _Ready()
    {
        base._Ready();
        Parent = Misc.findParent(this);
    }
}

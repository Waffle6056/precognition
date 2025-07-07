using Godot;
using System;

public partial class GreatswordSideslash : Action, IAnimated 
{
	[Export]
    public VisualManager Animation{get; set;}
	String AttackName = "Sideslash";
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	protected override bool StartAction()
	{
		if (!base.StartAction())
		 	return false;
		Animation.Play(AttackName);
		return true;
	}
}

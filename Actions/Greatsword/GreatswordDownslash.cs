using Godot;
using System;
using System.Collections.Generic;

public partial class GreatswordDownslash : Action, IAttack, IAnimated
{

	[Export]
    public AttackManager Attack{get;set;}
	[Export]
    public AnimationManager Animation{get; set;}
	String AttackName = "Swing";
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	protected override bool StartAction()
	{
		if (!base.StartAction())
		 	return false;
		Animation.Play(AttackName);
		return true;
	}
}

using Godot;
using System;
using System.Collections.Generic;

public partial class Greatsword : Action
{

	String AttackName = "Swing";
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);
	}
    protected override void StartChannel()
    {
        base.StartChannel();
		ActionAnimator.Play(AttackName);
    }
	public void AnimationEnd(String name)
	{
		if (Acting)
			ActionAnimator.PlayBackwards(AttackName);
		EndAction();		
	}
	
}

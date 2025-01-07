using Godot;
using System;
using System.Collections.Generic;

public partial class Greatsword : Weapon
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
		AttackAnimator.Play(AttackName);
    }
	public void AnimationEnd(String name)
	{
		if (Attacking)
			AttackAnimator.PlayBackwards(AttackName);
		EndAttack();		
	}
	
}

using Godot;
using System;
using System.Collections.Generic;

public partial class GreatswordDownslash : AttackAction
{

	String AttackName = "Swing";
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	protected override bool StartAction()
	{
		if (!base.StartAction())
		 	return false;
		ActionAnimator.Play(AttackName);
		return true;
	}

	public override void DealDamage(Node3D body){
        if (IsActing)
			base.DealDamage(body);
    }
}

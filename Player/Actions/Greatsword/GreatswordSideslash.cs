using Godot;
using System;

public partial class GreatswordSideslash : AttackAction
{
	String AttackName = "Sideslash";
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

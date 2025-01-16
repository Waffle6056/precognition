using Godot;
using System;
using System.Collections.Generic;

public partial class TailWhip : WeightedAttackAction
{

    String AttackName = "TailWhip";
    protected override void StartAction()
    {
        base.StartAction();
        GlobalBasis = Basis.LookingAt(Player.Instance.GlobalPosition-Root.TargetPos);
        ActionAnimator.Play(AttackName);
    }
    public void AnimationEnd(String name)
	{
		EndAction();		
	}
    public override double GetWeight()
    {
        return ExponentialFalloff(BaseWeight,WeightMultiplier,PlayerDistance(Root.TargetPos));
    }
}

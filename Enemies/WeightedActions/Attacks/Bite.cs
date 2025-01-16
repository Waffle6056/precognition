using Godot;
using System;
using System.Collections.Generic;

public partial class Bite : WeightedAttackAction
{
    
    [Export]
    public Vector3 Direction;
    String AttackName = "Bite";
    protected override void StartAction()
    {
        base.StartAction();
        GlobalBasis = Basis.LookingAt(-Direction);
        ActionAnimator.Play(AttackName);
    }
    public void AnimationEnd(String name)
	{
		EndAction();		
	}
    public override double GetWeight()
    {
        return LinearFalloff(BaseWeight, WeightMultiplier, PlayerDistance(Root.TargetPos + Direction));
    }
}

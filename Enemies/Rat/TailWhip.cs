using Godot;
using System;
using System.Collections.Generic;

public partial class TailWhip : WeightedAction
{
    [Export]
    public float BaseWeight = 50f;

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
        float DistanceFromEnd = Root.TargetPos.DistanceTo(Player.Instance.TargetPos);
        return BaseWeight + WeightMultiplier * -DistanceFromEnd * DistanceFromEnd;
    }
}

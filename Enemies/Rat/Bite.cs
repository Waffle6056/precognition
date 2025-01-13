using Godot;
using System;
using System.Collections.Generic;

public partial class Bite : WeightedAction
{
    [Export]
    public float BaseWeight = 50f;
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
        float DistanceFromEnd = (Root.TargetPos+Direction.Normalized()).DistanceTo(Player.Instance.TargetPos);
        if (DistanceFromEnd > 1)
            return -1;
        return BaseWeight + WeightMultiplier * -DistanceFromEnd;
    }
}

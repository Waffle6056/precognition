using Godot;
using System;
using System.Collections.Generic;

public partial class RatBite : WeightedWeapon
{
    [Export]
    public float BaseWeight = 50f;
    [Export]
    public Vector3 Direction;
    String AttackName = "Bite";
    protected override void StartAttack()
    {
        base.StartAttack();
        GlobalBasis = Basis.LookingAt(-Direction);
        AttackAnimator.Play(AttackName);
    }
    public void AnimationEnd(String name)
	{
		EndAttack();		
	}
    public override double GetWeight()
    {
        float DistanceFromEnd = (Root.TargetPos+Direction.Normalized()).DistanceTo(Player.Instance.TargetPos);
        if (DistanceFromEnd > 1)
            return -1;
        return BaseWeight + WeightMultiplier * -DistanceFromEnd;
    }
}

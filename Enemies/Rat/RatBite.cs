using Godot;
using System;
using System.Collections.Generic;

public partial class RatBite : WeightedWeapon
{
    protected override void StartAttack()
    {
        base.StartAttack();
        AttackAnimator.Play("Bite");
    }
    public override double GetWeight()
    {
        return 1/((Root.GlobalPosition.DistanceTo(Player.Instance.GlobalPosition)/10)+.9);
    }
}

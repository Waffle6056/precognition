using Godot;
using System;
using System.Collections.Generic;

public partial class TailWhip : Action, IWeighted, IAttack, IAnimated
{
    [Export]
    public AnimationManager Animation{get; set;}
    [Export]
    public AttackManager Attack{get;set;}
    [Export]
    public WeightProperties WeightProperties{get; set;}
    String AttackName = "TailWhip";
    protected override bool StartAction()
    {
        if (!base.StartAction())
            return false;
        GlobalBasis = Basis.LookingAt(Player.Instance.GlobalPosition-Properties.Root.TargetPos);
        Animation.Play(AttackName);
        return true;
    }
    public void AnimationEnd(String name)
	{
		EndAction();		
	}
    public double GetWeight()
    {
        return IWeighted.ExponentialFalloff(WeightProperties.BaseWeight,WeightProperties.WeightMultiplier,IWeighted.PlayerDistance(Properties.Root.TargetPos));
    }
}

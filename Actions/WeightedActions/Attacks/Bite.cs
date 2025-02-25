using Godot;
using System;
using System.Collections.Generic;

public partial class Bite : Action, IWeighted, IAttack, IAnimated
{
    [Export]
    public AnimationManager Animation{get; set;}
    [Export]
    public AttackManager Attack{get;set;}
    [Export]
    public WeightProperties WeightProperties{get; set;}
    [Export]
    public Vector3 Direction;
    String AttackName = "Bite";
    protected override bool StartAction()
    {
        if (!base.StartAction())
            return false;
        Animation.Play(AttackName);
        return true;
    }
    public void AnimationEnd(String name)
	{
        if (IsActing)
		    EndAction();		
	}
    public double GetWeight()
    {
        return IWeighted.LinearFalloff(WeightProperties.BaseWeight, WeightProperties.WeightMultiplier, IWeighted.PlayerDistance(Properties.Root.TargetPos + Direction));
    }
}

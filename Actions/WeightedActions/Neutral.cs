using Godot;
using System;

public partial class Neutral : Action, IWeighted
{
    [Export]
    public WeightProperties WeightProperties{get; set;}
    protected override bool StartAction()
    {
        if (!base.StartAction())
            return false;
		EndAction();
        return true;
    }

    public double GetWeight()
    {
        return WeightProperties.BaseWeight;
    }

}

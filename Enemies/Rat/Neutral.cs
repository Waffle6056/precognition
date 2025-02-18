using Godot;
using System;

public partial class Neutral : WeightedAction
{
    protected override bool StartAction()
    {
        if (!base.StartAction())
            return false;
		EndAction();
        return true;
    }

    public override double GetWeight()
    {
        return BaseWeight;
    }

}

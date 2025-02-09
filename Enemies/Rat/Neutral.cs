using Godot;
using System;

public partial class Neutral : WeightedAction
{
    protected override void StartAction()
    {
        base.StartAction();
		EndAction();
    }

    public override double GetWeight()
    {
        return BaseWeight;
    }

}

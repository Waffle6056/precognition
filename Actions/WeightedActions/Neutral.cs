using Godot;
using System;

public partial class Neutral : Action, IConstantWeighted
{
    [Export]
    public ConstantWeightManager WeightManager { get; set; }
    public double GetWeight()
    {
        return WeightManager.GetWeight();
    }
    public WeightManager GetWeightManager()
    {
        return WeightManager;
    }
    protected override bool StartAction()
    {
        if (!base.StartAction())
            return false;
		EndAction();
        return true;
    }

    

}

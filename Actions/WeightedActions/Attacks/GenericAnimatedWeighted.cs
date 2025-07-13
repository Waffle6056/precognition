using Godot;
using System;
using System.Collections.Generic;

public partial class GenericAnimatedWeighted : GenericAnimatedAction, IOffsetFalloff
{
    [Export]
    public Vector3 LocalOffset { get; set; }
    [Export]
    public OffsetLinearFalloff WeightManager { get; set; }
    public WeightManager GetWeightManager()
    {
        return WeightManager;
    }
    public double GetWeight(Entity root)
    {
        return WeightManager.CalculateWeight(root.GlobalTransform * LocalOffset);
    }

}


using Godot;
using System;

[GlobalClass]
public partial class OffsetLinearFalloff : WeightManager
{
    public double CalculateWeight(Vector3 Offset)
    {
        return LinearFalloff(WeightProperties.BaseWeight, WeightProperties.WeightMultiplier, PlayerDistance(Offset));
    }

}
interface IOffsetFalloff : IWeighted
{
    [Export]
    public OffsetLinearFalloff WeightManager { get; set; }
    [Export]
    public Vector3 LocalOffset { get; set; }
    //public WeightManager GetWeightManager()
    //{
    //    return WeightManager;
    //}
    //public double GetWeight()
    //{
    //    return WeightManager.GetWeight(ActionProperties.Root.GlobalTransform * LocalOffset);
    //}

}
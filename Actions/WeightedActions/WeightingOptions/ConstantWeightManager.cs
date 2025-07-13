using Godot;
using System;

public partial class ConstantWeightManager : WeightManager
{
    public double CalculateWeight()
    {
        return WeightProperties.BaseWeight;
    }
}
interface IConstantWeighted : IWeighted
{
    [Export]
    public ConstantWeightManager WeightManager { get; set; }
    //public WeightManager GetWeightManager()
    //{
    //    return WeightManager;
    //}
    //public double GetWeight()
    //{
    //    return WeightManager.GetWeight();
    //}
}

using Godot;
using System;
using System.Collections.Generic;

public abstract partial class WeightManager : Node
{
    [Export]
    public WeightProperties WeightProperties { get; set; }
    public static double LinearFalloff(float BaseWeight, float WeightMultiplier, float x)
    {
        if (x < 0)
            return -1;
        return BaseWeight - WeightMultiplier * x;
    }
    public static double ExponentialFalloff(float BaseWeight, float WeightMultiplier, float x)
    {
        if (x < 0)
            return -1;
        return BaseWeight - WeightMultiplier * x * x;
    }
    public static float PlayerDistance(Vector3 Pos)
    {
        //GD.Print(Pos + " " + Player.Instance);
        return Pos.DistanceTo(Player.Instance.TargetPos.GlobalPosition);
    }
}
interface IWeighted
{
    public WeightManager GetWeightManager();
    //public WeightManager GetWeightManager()
    //{
    //    return WeightManager;
    //}
    public double GetWeight(Entity root);

}

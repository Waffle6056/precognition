
using Godot;
using System;
using System.Collections.Generic; 
public abstract partial class WeightedAction : Action
{   
    [Export]
    public float BaseWeight = 50f;
    [Export]
    public float WeightMultiplier = 1;
    [Export]
    public WeightedAction[] FollowUpOptions;
    public abstract double GetWeight();
    public override void _Ready()
	{

	}
    public static double LinearFalloff(float BaseWeight, float WeightMultiplier, float x){
        if (x < 0)
            return -1;
        return BaseWeight - WeightMultiplier * x;
    }
    public static double ExponentialFalloff(float BaseWeight, float WeightMultiplier, float x){
        if (x < 0)
            return -1;
        return BaseWeight - WeightMultiplier * x * x;
    }
    public static float PlayerDistance(Vector3 Pos){
        return Pos.DistanceTo(Player.Instance.Position);
    }   

}

using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class WeightProperties : Node
{
	[Export]
	public float BaseWeight{get;set;}
	[Export]
	public float WeightMultiplier{get;set;}
	[Export]
	public Option[] FollowUpOptions{get;set;}
}
interface IWeighted
{
	[Export]
	public WeightProperties WeightProperties{get; set;}
	public double GetWeight();
	
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

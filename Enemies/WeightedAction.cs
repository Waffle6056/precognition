
using Godot;
using System;
using System.Collections.Generic; 
public abstract partial class WeightedAction : Action
{   
    [Export]
    public float WeightMultiplier = 1;
    public abstract double GetWeight();
    public override void _Ready()
	{
        if (Root is Enemy)
            (Root as Enemy).Options.Add(this);
	}
}
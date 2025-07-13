using Godot;
using System;

[GlobalClass]
public partial class WeightProperties : Node
{
    [Export]
    public float BaseWeight { get; set; }
    [Export]
    public float WeightMultiplier { get; set; }
    
}

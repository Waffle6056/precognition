using Godot;
using System;

public partial class BulletTime : Node
{
    public static float SpeedScale = 1f;
    [Export]
    public float BaseSpeedScale = 1f;
    [Export]
    public float SlowedSpeedScale = .1f;
    public static int outsideActivations = 0;
    [Export]
    public float BlurringPercentVelocity = 1f;
    public float BlurringPercent = 0;
    [Export]
    public ColorRect BlurNode;
    private ShaderMaterial _BlurMaterial { get { return BlurNode.Material as ShaderMaterial; } }
    public override void _Process(double delta)
    {
        base._Process(delta);
        if (Input.IsActionPressed("TestBind") || outsideActivations > 0)
        {
            GD.Print("called");
            SpeedScale = SlowedSpeedScale;
            BlurringPercent = (float)Math.Min(BlurringPercent + BlurringPercentVelocity * delta, 1);
        }
        else
        {
            SpeedScale = BaseSpeedScale;
            BlurringPercent = (float)Math.Max(BlurringPercent - BlurringPercentVelocity * delta, 0);
        }
        _BlurMaterial.SetShaderParameter("blurring_percent", BlurringPercent);
    }
}

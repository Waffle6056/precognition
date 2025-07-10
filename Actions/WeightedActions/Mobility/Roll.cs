using Godot;
using System;

public partial class Roll : Action, IOffsetFalloff, IAnimated, ITrackingChange
{
    [Export]
    public Vector3 LocalOffset { get; set; }
    [Export]
    public OffsetLinearFalloff WeightManager { get; set; }
    public WeightManager GetWeightManager()
    {
        return WeightManager;
    }
    public double GetWeight()
    {
        return WeightManager.GetWeight(ActionProperties.Root.GlobalTransform * LocalOffset);
    }

    [Export]
    public VisualManager Animation { get; set; }
    [Export]
    public TrackingProperties TrackingProperties { get; set; }
    [Export]
    public String AttackName = "Ball";
    [Export]
    public float Distance = 2f;
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    protected override bool StartChannel()
    {
        if (!base.StartAction())
            return false;
        Animation?.Play(AttackName);
        return true;
    }

    protected override bool Act(double delta)
    {
        if (!base.Act(delta))
            return false;
        CharacterBody3D TargetPos = ActionProperties.Root.TargetPos;
        Vector3 GlobalDirection = (TargetPos.GlobalBasis * LocalOffset.Normalized() * new Vector3(1, 0, 1)).Normalized();

        TargetPos.MoveAndCollide(GlobalDirection * Distance / ActionProperties.ActingMaximumTime * (float)delta);
        return true;
    }
}

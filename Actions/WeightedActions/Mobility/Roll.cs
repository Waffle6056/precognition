using Godot;
using System;

public partial class Roll : Action, IOffsetFalloff, IAnimated, ITrackingChange
{
    [Export]
    public virtual Vector3 LocalOffset { get; set; }
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
        if (!base.StartChannel())
            return false;
        Animation?.Play(AttackName);
        return true;
    }
    protected override bool StartAction()
    {
        if (!base.StartAction())
            return false;
        AddVelocity();
        return true;
    }

    public void AddVelocity()
    {
        CharacterBody3D TargetPos = ActionProperties.Root.TargetPos;
        Vector3 GlobalDirection = (TargetPos.GlobalBasis * LocalOffset.Normalized()).Normalized();

        TargetPos.Velocity += (GlobalDirection * Distance / ActionProperties.ActingMaximumTime);
    }
    public override void _PhysicsProcess(double delta)
    {
        
        base._PhysicsProcess(delta);
        //GD.Print(IsActing);
        if (!IsActing)
            return;

        AddVelocity();
    }
}

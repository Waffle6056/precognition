using Godot;
using System;
using System.Collections.Generic;

public partial class Dash : WeightedWeapon
{
    [Export]
    public Vector3 Direction;
    [Export]
    public float Distance;
    [Export]
    public float TotalTime;
    float Time;
    Vector3 StartPos;
    protected override void StartAttack()
    {
        base.StartAttack();
        StartPos = Root.TargetPos;
        Time = 0;
    }
    protected override void Attack(double delta)
    {
        base.Attack(delta);
        Time += (float) delta;
        if (Time > TotalTime)
        {
            EndAttack();
            Time = TotalTime;
        }
        Root.TargetPos = StartPos.Lerp(StartPos+Direction.Normalized()*Distance, Time/TotalTime);
    }
    public override double GetWeight()
    {
        if (Root.GridSpace.TestMove(new Transform3D(Root.GridSpace.GlobalBasis, Root.TargetPos+Direction*Distance),Vector3.Zero))
            return -1;
        float DistanceFromStart = Root.TargetPos.DistanceTo(Player.Instance.TargetPos);
        float DistanceFromEnd = (Root.TargetPos+Direction.Normalized()*Distance).DistanceTo(Player.Instance.TargetPos);
        if (DistanceFromEnd-DistanceFromStart > Distance)
            return -1;
        return WeightMultiplier * (1/(DistanceFromEnd+0.09f));
    }
}

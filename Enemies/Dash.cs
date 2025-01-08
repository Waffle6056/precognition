using Godot;
using System;
using System.Collections.Generic;

public partial class Dash : WeightedWeapon
{
    [Export]
    public float BaseWeight = 50f;
    [Export]
    public Vector3 Direction;
    [Export]
    public int BaseDistance = 4;
    [Export]
    public int MaxDistance = 8;
    [Export]
    public float TotalTime = .09f;
    float Time;
    Vector3 StartPos;
    Vector3 EndPos;
    protected override void StartAttack()
    {
        base.StartAttack();
        StartPos = Root.TargetPos;
        EndPos = StartPos+Direction.Normalized()*calcDistance();
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
        Root.TargetPos = StartPos.Lerp(EndPos, Time/TotalTime);
    }
    int calcDistance(){
        KinematicCollision3D Collider = new KinematicCollision3D();
        Vector3 End = Root.TargetPos;
        for (int i = 0; i < BaseDistance; i++)
        {
            if (Root.GridSpace.TestMove(new Transform3D(Root.GridSpace.GlobalBasis, End), Direction, Collider, default, default, 7)){
                
                for (int id = 0; id < Collider.GetCollisionCount(); id++){
                    //GD.Print(Collider.GetCollider(id));
                    if (!(Collider.GetCollider(id) is AnimatableBody3D ))
                        return -1;
                }
            }
            End += Direction.Normalized();
        }
        int j = 0;
        while (Collider.GetCollisionCount() > 0 && BaseDistance+j < MaxDistance)
        {
            if (Root.GridSpace.TestMove(new Transform3D(Root.GridSpace.GlobalBasis, End), Direction, Collider, default, default, 7)){
            //GD.Print(Collider.GetCollider());
                for (int id = 0; id < Collider.GetCollisionCount(); id++){
                    //GD.Print(Collider.GetCollider(id));
                    if (!(Collider.GetCollider(id) is AnimatableBody3D ))
                        return -1;
                }
            }
            End += Direction.Normalized();
            j++;
        }
        j = Math.Max(0,j-1);
        //GD.Print(j);
        return BaseDistance+j;
    }

    public override double GetWeight()
    {
        int d = calcDistance();
        if (d < 0)
            return -1;
        float DistanceFromStart = Root.TargetPos.DistanceTo(Player.Instance.TargetPos);
        float DistanceFromEnd = (Root.TargetPos+Direction.Normalized()*d).DistanceTo(Player.Instance.TargetPos);
        if (DistanceFromEnd-DistanceFromStart > BaseDistance)
            return -1;
        return BaseWeight + WeightMultiplier * -DistanceFromEnd;
    }
}

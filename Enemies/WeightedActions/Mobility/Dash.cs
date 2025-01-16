using Godot;
using System;
using System.Collections.Generic;

public partial class Dash : WeightedAction
{
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
    protected override void StartAction()
    {
        base.StartAction();
        StartPos = Root.TargetPos;
        EndPos = StartPos+Direction.Normalized()*calcDistance();
        Time = 0;
    }
    protected override void Act(double delta)
    {
        base.Act(delta);
        Time += (float) delta;
        if (Time > TotalTime)
        {
            EndAction();
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
                    if (!(Collider.GetCollider(id) is Entity ))
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
                    if (!(Collider.GetCollider(id) is Entity ))
                        return -1;
                }
            }
            End += Direction.Normalized();
            j++;
        }
        if (Collider.GetCollisionCount() > 0)
            return -1;
        j = Math.Max(0,j-1);
        //GD.Print(j);
        return BaseDistance+j;
    }

    public override double GetWeight()
    {
        int d = calcDistance();
        if (d < 0)
            return -1;
        float DistanceFromStart = PlayerDistance(Root.TargetPos);
        float DistanceFromEnd = PlayerDistance(Root.TargetPos+Direction.Normalized()*d);
        if (DistanceFromEnd-DistanceFromStart > BaseDistance-1)
            return -1;
        return LinearFalloff(BaseWeight,WeightMultiplier,DistanceFromEnd);
    }
}

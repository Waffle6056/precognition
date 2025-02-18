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
    double CallTime = 0.0;
    Vector3 StartPos;
    Vector3 EndPos;
    protected override bool StartAction()
    {

        if (!base.StartAction())
		 	return false;
        StartPos = Root.TargetPos;
        int distance = calcDistance();
        if (distance < 0){
            EndAction();
            return false;
        }
        EndPos = StartPos+Direction*distance;
        CallTime = ActionTime;
        return true;
    }
    protected override bool Act(double delta)
    {
        if (!base.Act(delta))
            return false;
        if (ActionTime-CallTime > TotalTime){
            CallTime = ActionTime-TotalTime;
            EndAction();
        }
        
        Root.TargetPos = StartPos.Lerp(EndPos, (float)((ActionTime-CallTime)/TotalTime));
        return true;
    }
    int calcDistance(){
        KinematicCollision3D Collider = new KinematicCollision3D();
        Vector3 End = Root.TargetPos;
        for (int i = 0; i < BaseDistance; i++)
        {
            if (Root.GridSpace.TestMove(new Transform3D(Root.GridSpace.GlobalBasis, End), Direction, Collider, default, default, 19)){
                
                for (int id = 0; id < Collider.GetCollisionCount(); id++){
                    //GD.Print(Collider.GetCollider(id));
                    if (!(Collider.GetCollider(id) is AnimatableBody3D ))
                        return -1;
                }
            }
            End += Direction;
        }
        int j = 0;
        while (Collider.GetCollisionCount() > 0 && BaseDistance+j < MaxDistance)
        {
            if (Root.GridSpace.TestMove(new Transform3D(Root.GridSpace.GlobalBasis, End), Direction, Collider, default, default, 19)){
            //GD.Print(Collider.GetCollider());
                for (int id = 0; id < Collider.GetCollisionCount(); id++){
                    //GD.Print(Collider.GetCollider(id));
                    if (!(Collider.GetCollider(id) is AnimatableBody3D ))
                        return -1;
                }
            }
            End += Direction;
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
        float DistanceFromEnd = PlayerDistance(Root.TargetPos+Direction*d);
        if (DistanceFromEnd-DistanceFromStart > BaseDistance-1)
            return -1;
        return LinearFalloff(BaseWeight,WeightMultiplier,Math.Min(DistanceFromEnd,DistanceFromStart));
    }
}

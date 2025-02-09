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
        int distance = calcDistance();
        if (distance < 0){
            EndAction();
            return;
        }
        EndPos = StartPos+Direction*distance;
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
            if (Root.GridSpace.TestMove(new Transform3D(Root.GridSpace.GlobalBasis, End), Direction, Collider, default, default, 19)){
                
                for (int id = 0; id < Collider.GetCollisionCount(); id++){
                    //GD.Print(Collider.GetCollider(id));
                    if (!(Collider.GetCollider(id) is Entity ))
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
                    if (!(Collider.GetCollider(id) is Entity ))
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


    public new int DataLength{get{return base.DataLength+1;}}
    public override List<Object> GetData()
    {		
        List<Object> data = base.GetData();
		data.AddRange(new List<Object>
        {
            Time,
        });
		return data;
    }

    public override void SetData(List<Object> data)
    {
		base.SetData(data);
		Time = (float) data[base.DataLength+0];
    }
}

using Godot;
using System;
using System.Collections.Generic;

public partial class Burrow : WeightedAction
{
    [Export]
    public float Radius = 4f;
    String AttackName = "Burrow";
    static Random randomSeedGen = new Random();
    int randomSeed = 1;
    public override void _Ready()
	{
        base._Ready();
        randomSeed = randomSeedGen.Next();
    }
    //static Random randSeed = new Random()
    protected override bool StartChannel()
    {
        if (!base.StartChannel())
            return false;
		ActionAnimator.Play(AttackName);
        return true;
    }
    int adj(double val){
        return (int)((val-0.5) * 2 * Radius);
    }
    protected override bool StartAction()
    {
        if (!base.StartAction())
            return false;
        if (Math.Abs(Radius) == 1)
            return false;

        int count = 0;
        Vector3 start = Root.TargetPos;
        do {
            Random rand = new Random(randomSeed);
            Root.TargetPos = Player.Instance.TargetPos + new Vector3(adj(rand.NextDouble()),0,adj(rand.NextDouble()));
            Root.GridSpace.GlobalPosition = Root.TargetPos;
            
            count++;
            randomSeed += new Random(randomSeed).Next();
        } while (count < Radius*Radius*90 && Root.GridSpace.TestMove(new Transform3D(Root.GridSpace.GlobalBasis,Root.TargetPos+Vector3.Down),Vector3.Up));
        if (Root.GridSpace.TestMove(new Transform3D(Root.GridSpace.GlobalBasis,Root.TargetPos+Vector3.Down),Vector3.Up))
            Root.TargetPos = start;
       //GD.Print(Root.TargetPos);
       return true;
    }
    public void AnimationEnd(String name)
	{
		EndAction();
	}
    public override double GetWeight()
    {
        return BaseWeight;
    }


    public new int DataLength{get{return base.DataLength+1;}}
    public override List<Object> GetData()
    {		
        List<Object> data = base.GetData();
		data.AddRange(new List<Object>
        {
            randomSeed,
        });
		return data;
    }

    public override void SetData(List<Object> data)
    {
		base.SetData(data);
		randomSeed = (int) data[base.DataLength+0];
    }
}

using Godot;
using System;
using System.Collections.Generic;

public partial class Burrow : Action, IWeighted, IAnimated
{
    [Export]
    public WeightProperties WeightProperties{get; set;}
    [Export]
    public AnimationManager Animation{get; set;}
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
		Animation.Play(AttackName);
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
        Vector3 start = Properties.Root.TargetPos;
        do {
            Random rand = new Random(randomSeed);
            Properties.Root.TargetPos = Player.Instance.TargetPos + new Vector3(adj(rand.NextDouble()),0,adj(rand.NextDouble()));
            Properties.Root.GridSpace.GlobalPosition = Properties.Root.TargetPos;
            
            count++;
            randomSeed += new Random(randomSeed).Next();
        } while (count < Radius*Radius*90 && Properties.Root.GridSpace.TestMove(new Transform3D(Properties.Root.GridSpace.GlobalBasis,Properties.Root.TargetPos+Vector3.Down),Vector3.Up));
        if (Properties.Root.GridSpace.TestMove(new Transform3D(Properties.Root.GridSpace.GlobalBasis,Properties.Root.TargetPos+Vector3.Down),Vector3.Up))
            Properties.Root.TargetPos = start;
       //GD.Print(Root.TargetPos);
       return true;
    }
    public void AnimationEnd(String name)
	{
		EndAction();
	}
    public double GetWeight()
    {
        return WeightProperties.BaseWeight;
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

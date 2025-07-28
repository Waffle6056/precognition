using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class CooldownManager : Node, RewindableObject
{
	[Export]
	public float CDBase = 0;
	protected double CD = 0;

	public bool OnCD() { return CD > 0; }
	public virtual void Start()
	{
		CD = CDBase;
	}
	public virtual void End()
	{
		;
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

		CD -= delta * BulletTime.SpeedScale;
	}


	public int DataLength{get{return 1;}}
	public virtual List<Object> GetData()
	{
		List<Object> data = new List<Object>
		{
			CD,
		};
		return data;
	}

	public virtual void SetData(List<Object> data)
	{
		CD = (double) data[0];		
	}
}

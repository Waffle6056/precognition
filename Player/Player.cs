using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
public partial class Player : Entity
{
	[Export]
	public float HoldMovementCD = 0.15f;

	public static Player Instance {get; private set;}
	public override void _Ready()
	{
		base._Ready();
		Instance = this;
	}

	protected double[] CD = new double[4];
	protected bool[] Pressed = new bool[4];
	protected virtual Vector3 checkMovement(String k, String op, int ind, Vector3 vec){
		if (Input.IsActionJustPressed(k) || Input.IsActionPressed(k) && !Pressed[(ind+2)%4])
		{
			Pressed[ind] = true;
			Pressed[(ind+2)%4] = false;
		}

		if (!Input.IsActionPressed(k))
		{
			Pressed[ind] = false;
			Pressed[(ind+2)%4] = Input.IsActionPressed(op);
		}
			
		
		if (RewindController.Instance.IsPaused && !RewindController.Instance.IsFateing)
			return Vector3.Zero;

		if ((Input.IsActionJustPressed(k) || Pressed[ind] && CD[ind] <= 0) && !GridSpace.TestMove(new Transform3D(GridSpace.GlobalBasis,TargetPos),vec))
		{
			TargetPos += vec;
			CD[ind] = HoldMovementCD;
			return vec;
		}

		return Vector3.Zero;
	}
	
	private void MoveTarget(double delta)
	{
		for (int i = 0; i < 4; i++)
			CD[i] -= delta;

		checkMovement("Up",   "Down", 0,Vector3.Forward);
		checkMovement("Left", "Right",1,Vector3.Left);
		checkMovement("Down", "Up",   2,Vector3.Back);
		checkMovement("Right","Left", 3,Vector3.Right);
		
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);
		if (!Channelling && !Acting)
			MoveTarget(delta);		
	}




    public override void SetData(List<object> data)
    {
		if (RewindController.Instance.IsFateing && !RewindController.Instance.IsRewinding)
			return;
		base.SetData(data);
    }


}

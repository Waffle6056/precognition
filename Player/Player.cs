using Godot;
using System;
using System.Collections.Generic;
public partial class Player : Node3D, RewindableObject
{
	[Export]
	public float MaxHP = 100;
	[Export]
	public float CurrentHP = 100;
	[Export]
	public ColorRect VisualHP;
	[Export]
	public bool LerpOn = true;
	[Export]
	public float Weight = .5f;
	[Export]
	public float HoldMovementCD = 0.15f;
	public bool Channelling = false;
	public bool Attacking = false;

	public static Player Instance {get; private set;}
	public override void _Ready()
	{
		Instance = this;
	}

	public Vector3 TargetPos = new Vector3(0,0,0);
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
			
			
		if ((Input.IsActionJustPressed(k) || Pressed[ind] && CD[ind] <= 0) && !RewindController.Instance.IsPaused)
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
		if (!Channelling && !Attacking)
			MoveTarget(delta);
		if (LerpOn)
			Position = Position.Lerp(TargetPos,Weight);
		else
			Position = TargetPos;
			
		VisualHP.Size = new Vector2(CurrentHP,40);
	}



    public List<Object> GetData()
    {
        List<Object> data = new List<Object>
        {
            TargetPos,
            CurrentHP,
            MaxHP,
			Attacking,
			Channelling
        };
		return data;
    }

    public void SetData(List<Object> data)
    {
		TargetPos   = (Vector3) data[0];
		CurrentHP   = (float)   data[1];
		MaxHP       = (float)   data[2];
		Attacking   = (bool)    data[3];
		Channelling = (bool)    data[4];
    }

	


}

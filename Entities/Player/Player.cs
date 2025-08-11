	using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

public partial class Player : Entity
{
	[Export]
	public TargetFollowCamera CurrentCamera = null;
	[Export]
	public Roll SlideRoll;
	[Export]
	public Roll SidestepRoll;
    [Export]
    public Jump Knockback;
    [Export]
	public Jump Jump;
	[Export]
	public float DashBufferTime = .2f;
	private double currentDashBufferTime = 0;
	public bool IsDashing { get { return SlideRoll.Active || SidestepRoll.Active; } }
	public bool IsRunning = false;

	public static Player Instance {get; private set;}
	[Export]
	public Limb HeadBox;
	[Export]
	public Limb LegBox;

	enum State
	{
		Crouching,
		Standing,
		Airborne
	}
	State PlayerState = State.Standing;
	[Export]
	public float CrouchingSpeedRatio = 2/3f;
	[Export]
	public float WalkingSpeed = 3f;
	[Export]
	public float RunningSpeed = 4f;

	public float TargetSpeed = 3f;

    

    public override void _Ready()
	{
		//GD.Print(" SET INSTANCE _-_--------------------");
		base._Ready();
		Instance = this;
		foreach (Option a in CurrentAction.ActionProperties.FollowUpOptions)
			Options.AddRange(a.GetActions());
		//GD.Print(Instance+" SET INSTANCE _-_--------------------");
	}

	public override void _Process(double delta)
	{
		//GD.Print(Input.IsActionPressed("Crouch") + " && " + Active + " || " + SlideRoll.Active+" = "+PlayerState);
		

		LegBox.Visible = LegBox.Monitorable = LegBox.Monitoring = PlayerState != State.Airborne;

		if (PlayerState == State.Crouching)
			HeadBox.Position = Vector3.Zero;
		else
			HeadBox.Position = new Vector3(0, .7f, 0);


		//Rotation = new Vector3(0, CurrentCamera.Rotation.Y, CurrentCamera.Rotation.Z);


		if (Active)
			CurrentCamera.Focused = true;
		else
			CurrentCamera.Focused = false;

		IsRunning = Input.IsActionPressed("Dash");

		

		InputAction();

		//GD.Print(DashCharges);
		base._Process(delta);
		//GD.Print("PARENT");
	}
	public override void _PhysicsProcess(double delta)
	{

		if (currentDashBufferTime > 0)
			currentDashBufferTime -= delta;
		if (Input.IsActionJustPressed("Dash"))
			currentDashBufferTime = DashBufferTime;
		processDash();
		MoveTarget();
		base._PhysicsProcess(delta);

		if (!TargetPos.IsOnFloor())
			PlayerState = State.Airborne;
		else if (Input.IsActionPressed("Crouch") || SlideRoll.Active)
			PlayerState = State.Crouching;
		else
			PlayerState = State.Standing;

		if (PlayerState != State.Airborne)
		{
			if (Jump.IsActing)
				Jump.Interrupt();
			if (Knockback.IsActing)
			{
				Knockback.Interrupt();
				InvulnTimeRemaining = .1f;
			}
		}
		//GD.Print(Animation.ActionAnimator.CurrentAnimation+" "+CurrentAction.Name);

	}


	Action next = null;
	List<Action> Options = new List<Action>();
	public void InputAction()
	{
		if (CurrentAction == null)
			return;

		for (int i = 1; i <= 5 && i-1 < Options.Count; i++)
			if (Input.IsActionJustPressed("AttackOption" + i))
				next = Options[i - 1];

		//GD.Print(next+" "+Options.Count+" "+CurrentAction.Name+" "+Active+" "+ CurrentAction.CurrentActionBufferTime);
		if (!Active && ( CurrentAction.CurrentActionBufferTime <= 0 || next != null )) // next action called when something is selected or the timer for selecting runs out
		{
			if (next == null)
				next = CurrentAction.ActionProperties.DefaultFollowUp;
			//GD.Print("Swap action to "+next.Name);
			(CurrentAction = next).CallAction();
			next = null;
			//GD.Print(CurrentAction.Name);

			Options.Clear();
			foreach (Option a in CurrentAction.ActionProperties.FollowUpOptions)
				Options.AddRange(a.GetActions());
		}
	}

	private void MoveTarget()
	{
		if (CurrentCamera == null)
		{
			GD.Print("NO CAMERA ASSIGNED");
			return;	
		}

		Vector3 globalMovement = inputDirection();

		TargetSpeed = IsRunning ? RunningSpeed : WalkingSpeed;
		if (PlayerState == State.Crouching)
			TargetSpeed *= CrouchingSpeedRatio;

		if (!IsActing && !IsLagging)
			TargetPos.Velocity += globalMovement * TargetSpeed;

		//GD.Print(Velocity.Length() + " " + Velocity.Y);
		if (!globalMovement.Length().Equals(Math.Abs(globalMovement.Y)))
		{
			//    TargetPos.GlobalBasis = Basis.LookingAt((movement * new Vector3(1, 0, 1)).Normalized());
			SlideRoll.LocalOffset = (globalMovement * CurrentCamera.GlobalBasis * new Vector3(1, 0, 1)).Normalized();
			SidestepRoll.LocalOffset = (globalMovement * CurrentCamera.GlobalBasis * new Vector3(1, 0, 1)).Normalized();
			//GD.Print(SidestepRoll.LocalOffset);
		}

	}

	protected override void postVelocityCalculation()
	{
		base.postVelocityCalculation();
		if (IsStunned)
			Velocity = Vector3.Zero;
	}


	private void processDash()
	{
		bool dashInputted = currentDashBufferTime > 0 && Input.IsActionJustReleased("Dash");
		bool dashValid = !IsDashing && !IsLagging && IsGrounded;
		//GD.Print(IsDashing + " " + Input.IsActionJustPressed("Dash") + " " + PlayerState);
		if (dashInputted && dashValid)
		{
			GD.Print("Dash Inputted");
			SidestepRoll.CallAction();
			if (Jump.Active)
			{
				GD.Print("Jump override");
				SidestepRoll.Interrupt();
				SidestepRoll.AddVelocity();
				Jump.SetVelocityParams(TargetPos.Velocity, RisingGravity);
			}
		}

		bool fastMovement = SidestepRoll.Active || IsRunning;
		//GD.Print(SidestepRoll.Active +" "+PlayerState);
		if (fastMovement && PlayerState == State.Crouching)
		{
			SidestepRoll.Interrupt();
			SlideRoll.CallAction();
		}
		//GD.Print(TargetPos.Velocity);
		
		if (Input.IsActionJustReleased("Jump") && IsGrounded)
		{
			GD.Print("Jumpped");
			SidestepRoll.Interrupt();
			SlideRoll.Interrupt();
			Jump.SetVelocityParams(TargetPos.Velocity, RisingGravity);
			AirTimeStart = EntityTime;
			Jump.CallAction();


		}
		
		
		//if (movement.Length() == 0)
		//    DashDirection = new Vector3(0, 0, 1);
		//if (!IsDashing && Input.IsActionJustPressed("Dash")) {
		//    //if (Input.IsActionJustPressed("PivotLeft"))
		//    //    movement = (CurrentCamera.TargetRotation[0] * new Vector3(1, 0, 1)).Normalized();
		//    //if (Input.IsActionJustPressed("PivotRight"))
		//    //    movement = -(CurrentCamera.TargetRotation[0] * new Vector3(1, 0, 1)).Normalized();

		//    if (IsDistorted)
		//    {


		//        InvulnTimeRemaining = DashLength;
		//        DashCallTime = EntityTime;
		//        DashDirection = movement;
		//        GD.Print(movement + " " + DashDirection);

		//    }
		//    else
		//    {
		//        DashDistance = SidestepDashDistance;

		//        InvulnTimeRemaining = DashLength;
		//        DashCallTime = EntityTime;
		//        DashDirection = movement;
		//    }
		//}


		//if (IsDashing)
		//    TargetPos.Velocity += (CurrentCamera.GlobalBasis * DashDirection * new Vector3(1,0,1)).Normalized() * DashDistance / DashLength;
	}
	
	//private void RewindBlink()
	//{
	//	Vector3 blinkPos = (Vector3) RewindController.Instance.Past.First.Value.StateData[this].Data[3];
	//	GlobalPosition = blinkPos;
	//	TargetPos.GlobalPosition = blinkPos;
	//}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	




	public override void SetData(List<object> data)
	{
		if (RewindController.Instance.IsFateing && !RewindController.Instance.IsRewinding)
			return;
		base.SetData(data);
	}

	public override Vector3 TakeKnockback(Vector3 Force)
	{
		////GD.Print("KNOCKBACK NOT IMPLEMENTED");
		//KinematicCollision3D col = MoveAndCollide(Force, true);
		////GD.Print(col);
		//if (col != null)
		//    TargetPos += col.GetTravel();
		//else
		//    TargetPos += Force;
		//GD.Print("KNOCKBACK FORCE " + Force);
		if (IsInvuln)
			return Vector3.Zero;
		//TargetPos.MoveAndCollide(Force);
		//GD.Print("called");
		//GD.Print(Force);
		Knockback.JumpDistance = Force.Y;
		Knockback.SetVelocityParams(Force, RisingGravity);
		Knockback.CallAction();
        return Force;
	}


	protected bool[] Pressed = new bool[4];
	String[] KeyNames = { "Up", "Left", "Down", "Right" };
	private Vector3 inputDirection()
	{
		for (int i = 0; i < 4; i++)
			checkEnabled(KeyNames[i], KeyNames[(i + 2) % 4], ind:i, oind:(i + 2) % 4);

		for (int i = 0; i < 4; i++)
			checkPressed(KeyNames[i], KeyNames[(i + 2) % 4], ind:i, oind:(i + 2) % 4);

		Vector3 forward = -CurrentCamera.GlobalBasis[2];
		forward.Y = 0;
		forward = forward.Normalized();
		Vector3 right = CurrentCamera.GlobalBasis[0];
		right.Y = 0;
		right = right.Normalized();

		Vector3[] dirs = { forward, -right, -forward, right };
		Vector3 movement = Vector3.Zero;
		for (int i = 0; i < 4; i++)
			movement += checkMovement(KeyNames[i], KeyNames[(i + 2) % 4], i, (i + 2) % 4, dirs[i]);
		return movement.Normalized();

	}
	protected virtual bool checkEnabled(String k, String op, int ind, int oind)
	{
		if (!Input.IsActionPressed(k))
		{
			Pressed[ind] = false;
			Pressed[oind] = Input.IsActionPressed(op);
			return false;
		}
		return true;
	}
	protected virtual bool checkPressed(String k, String op, int ind, int oind)
	{
		if (Input.IsActionJustPressed(k))
		{
			Pressed[ind] = true;
			Pressed[oind] = false;
			return false;
		}
		return true;
	}
	protected virtual Vector3 checkMovement(String k, String op, int ind, int oind, Vector3 vec)
	{


		if (RewindController.Instance.IsPaused && !RewindController.Instance.IsFateing)
			return Vector3.Zero;

		if (Pressed[ind])
		{
			return vec;
		}

		return Vector3.Zero;
	}


	
}

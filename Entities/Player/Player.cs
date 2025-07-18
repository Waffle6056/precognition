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
    public Jump Jump;
    [Export]
    public float ActionBufferTime = 1f;
    private double currentActionBufferTime = 0;
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

        

        InputAction(delta);

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
        MoveTarget(delta);
        base._PhysicsProcess(delta);

        if (!TargetPos.IsOnFloor())
            PlayerState = State.Airborne;
        else if (Input.IsActionPressed("Crouch") || SlideRoll.Active)
            PlayerState = State.Crouching;
        else
            PlayerState = State.Standing;

        if (PlayerState != State.Airborne && Jump.IsActing)
        {
            Jump.Interrupt();
        }
     

    }
    Action next = null;
    List<Action> Options = new List<Action>();
    public void InputAction(double delta)
    {
        if (CurrentAction == null)
            return;

        if (currentActionBufferTime > 0)
            currentActionBufferTime -= delta;
        if (CurrentAction.EndedActing)
            currentActionBufferTime = ActionBufferTime;



        for (int i = 1; i <= 5 && i-1 < Options.Count; i++)
            if (Input.IsActionJustPressed("AttackOption" + i))
                next = Options[i - 1];

        //GD.Print(next+" "+Options.Count+" "+CurrentAction.Name+" "+Active);
        if (!Active && ( currentActionBufferTime <= 0 || next != null )) // next action called when something is selected or the timer for selecting runs out
        {
            if (next == null)
                next = CurrentAction.ActionProperties.DefaultFollowUp;
            //GD.Print("Swap action to "+next.Name);
            (CurrentAction = next).CallAction(this);
            next = null;
            //GD.Print(CurrentAction.Name);

            Options.Clear();
            foreach (Option a in CurrentAction.ActionProperties.FollowUpOptions)
                Options.AddRange(a.GetActions());
        }
    }
    private void MoveTarget(double delta)
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
        
        //GD.Print(IsDashing + " " + Input.IsActionJustPressed("Dash") + " " + PlayerState);
        if (!IsDashing && currentDashBufferTime > 0 && Input.IsActionJustReleased("Dash") && PlayerState != State.Airborne && !IsLagging)
        {
            SidestepRoll.CallAction(this);
        }
        //GD.Print(SidestepRoll.Active +" "+PlayerState);
        if ((SidestepRoll.Active || IsRunning) && PlayerState == State.Crouching)
        {
            SidestepRoll.Interrupt();
            SlideRoll.CallAction(this);
        }
        //GD.Print(TargetPos.Velocity);
        
        if (Input.IsActionJustPressed("Jump") && PlayerState != State.Airborne)
        {
            SidestepRoll.Interrupt();
            SlideRoll.Interrupt();
            Jump.SetDistance(TargetPos.Velocity, RisingGravity);
            Jump.CallAction(this);


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
    public override float TakeHit(float Damage)
    {
        //if (IsInvuln)
        //    return 0;
        //CurrentEnergy -= Damage;
        //Animation?.HitStop(Damage * HitstunMul);
        //LastHitTime = EntityTime;
        //InvulnTimeRemaining = .1f;
        //return Damage;
        Damage = base.TakeHit(Damage);
        TakeStunned(Damage * HitstunMul); 
        InvulnTimeRemaining = .1f + Damage * HitstunMul;
        Animation?.HitStop(Damage * HitstunMul);
        return Damage;
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
        TargetPos.MoveAndCollide(Force);
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

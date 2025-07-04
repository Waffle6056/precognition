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
    public float TargetSpeed = 15f;
    [Export]
    public float DistortedDashDistance = 3f;
    [Export]
    public float SidestepDashDistance = 1f;
    public float DashDistance = 2f;
    [Export]
    public float DashLength = 1f;
    public double DashCallTime = -10;
    public Vector3 DashDirection;
    [Export]
    public float DistortedDashEnergyUsage = 2f;
    [Export]
    public float DistortedDashEnergyThreshold = 2f;
    public bool IsDashing { get { return EntityTime >= DashCallTime && EntityTime <= DashCallTime + DashLength; } }
    public bool IsDistorted = false;
    [Export]
    public int MaxDistortionCharges = 2;
    public int DistortionCharges = 2;
    [Export]
    public float DistortionChargeCooldown = 2f;
    public double DistortionRegenCalltime = 2f;

    [Export]
    public float JumpDistance = 2f;
    [Export]
    public float JumpLength = 2f;
    public double JumpCallTime = -10;
    public static Player Instance {get; private set;}
	public override void _Ready()
    {
        //GD.Print(" SET INSTANCE _-_--------------------");
        base._Ready();
		Instance = this;
        //GD.Print(Instance+" SET INSTANCE _-_--------------------");
	}

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("Distort"))
            IsDistorted = !IsDistorted;

        if (DistortionCharges >= MaxDistortionCharges)
            DistortionRegenCalltime = EntityTime;
        if (DistortionRegenCalltime + DistortionChargeCooldown < EntityTime)
        {
            DistortionCharges++;
            DistortionRegenCalltime = EntityTime;
        }
        //Rotation = new Vector3(0, CurrentCamera.Rotation.Y, CurrentCamera.Rotation.Z);
        MoveTarget(delta);

        if (Active)
            CurrentCamera.Focused = true;
        else
            CurrentCamera.Focused = false;
        
        //GD.Print(DashCharges);
        base._Process(delta);
        //GD.Print("PARENT");
    }

    private void MoveTarget(double delta)
	{
		if (CurrentCamera == null)
		{
			GD.Print("NO CAMERA ASSIGNED");
			return;	
		}

        Vector3 movement = inputDirection();
        
        TargetPos.Velocity += movement * TargetSpeed;

        //GD.Print(Velocity.Length() + " " + Velocity.Y);
        //if (!movement.Length().Equals(Math.Abs(movement.Y)))
        //    TargetPos.GlobalBasis = Basis.LookingAt((movement * new Vector3(1, 0, 1)).Normalized());
        

        processDash(movement);
        
        processJump();
    }

    
    private void processDash(Vector3 movement)
    {
        if (movement.Length() == 0)
            movement = (GlobalBasis[2] * new Vector3(1, 0, 1)).Normalized();
        if (!IsDashing && Input.IsActionJustPressed("Dash")) {
            //if (Input.IsActionJustPressed("PivotLeft"))
            //    movement = (CurrentCamera.TargetRotation[0] * new Vector3(1, 0, 1)).Normalized();
            //if (Input.IsActionJustPressed("PivotRight"))
            //    movement = -(CurrentCamera.TargetRotation[0] * new Vector3(1, 0, 1)).Normalized();

            if (IsDistorted)
            {
                if (DistortionCharges > 0 && CurrentEnergy > DistortedDashEnergyThreshold)
                {
                    DistortionCharges--;
                    TakeHit(DistortedDashEnergyUsage);
                    DashDistance = DistortedDashDistance;
                    IsDistorted = false;

                    DashCallTime = EntityTime;
                    DashDirection = movement;
                    
                }
            }
            else
            {
                DashDistance = SidestepDashDistance;

                DashCallTime = EntityTime;
                DashDirection = movement;
            }
        }


        if (IsDashing)
            TargetPos.Velocity += DashDirection * DashDistance / DashLength;
    }
    private void processJump()
    {
        //GD.Print("CHECKING JUMP "+ TargetPos.IsOnFloor());
        if (Input.IsActionJustPressed("Jump") && TargetPos.IsOnFloor())
        {
            JumpCallTime = EntityTime;
            GD.Print("CALLED JUMP");
        }
        if (EntityTime >= JumpCallTime && EntityTime <= JumpCallTime + JumpLength)
            TargetPos.Velocity += Vector3.Up * JumpDistance / JumpLength;


        //if (!IsOnFloor())
        //	TargetPos += new Vector3(0, -1, 0) * (float)delta;
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
        TargetPos.GlobalPosition += Force;
        return Force;
    }


    protected bool[] Pressed = new bool[4];
    String[] KeyNames = { "Up", "Left", "Down", "Right" };
    private Vector3 inputDirection()
    {
        for (int i = 0; i < 4; i++)
            checkEnabled(KeyNames[i], KeyNames[(i + 2) % 4], i, (i + 2) % 4);

        for (int i = 0; i < 4; i++)
            checkPressed(KeyNames[i], KeyNames[(i + 2) % 4], i, (i + 2) % 4);

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

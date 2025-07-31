using Godot;
using System;
using System.Collections.Generic;

public partial class Entity : CharacterBody3D, RewindableObject, ActionState, IAnimated
{
    [Export]
    TrackingProperties TrackingPropertiesBase;
    [Export]
	public float MaxEnergy{get; set;} = 100;
	[Export]
	public float CurrentEnergy{get; set;} = 100;
    public double LastHitTime  = 0;
    public double InvulnTimeRemaining = 0;
    [Export]
    public bool InvulnToggle = false;
    public bool IsInvuln { get { return InvulnToggle || InvulnTimeRemaining > 0; } }
    [Export]
    public float HitstunMul = .005f;
    [Export]
    public virtual VisualManager Animation { get; set; }
    [Export]
    public String AnimName { get; set; } = "Idle";
    [Export]
    public Vector3 ForesightOffset { get; set; }
    [Export]
    public CharacterBody3D TargetPos = null;

    [Export]
    public Node3D CenterOfMass;
    [Export]
    public Node3D CenterOfMassBase;
    [Export]
    public float SlowRecoveryWeight = 0.05f;
    [Export]
    public float QuickRecoveryWeight = 1f;
    [Export]
    public float StumbleStabilityThreshold = 1f;
    [Export]
    public float FallStabilityThreshold = 0.5f;
    [Export]
    public float StumbleStunDuration = 0.5f;
    [Export]
    public float FallStunDuration = 2f;
    [Export]
    public float FallMaxDistance = 2f;
    [Export]
    public Limb[] StartingLimbs;
    public List<Limb> Limbs = new List<Limb>();
    [Export]
    public int[] FallDirections;
    public float Stability = 1;
    [Export]
	public ColorRect VisualHP;
    [Export]
	public bool LerpOn = true;
    [Export]
	public float WalkLerpWeight = 10f;
    [Export]
    public float FallingGravity = -15f;
    [Export]
    public float RisingGravity = -10f;
    [Export]
    public float MaxWalkSpeed = 10f;
    [Export]    
    public float Speed = -1f;
    [Export]
    public float CoyoteTime = .2f;
    protected double AirTimeStart = 0f;
    public bool IsGrounded { get { return TargetPos.IsOnFloor() || EntityTime - AirTimeStart < CoyoteTime; } set { } }
    [Export]
    public Action CurrentAction;

    public bool IsChannelling{get{ return CurrentAction!=null && CurrentAction.IsChannelling; } set{}}
	public bool IsActing{get{ return CurrentAction!=null && CurrentAction.IsActing; } set{}}
	public bool IsLagging{get{ return CurrentAction!=null && CurrentAction.IsLagging; } set{} }
    public bool IsStunned{ get { return EntityTime >= StunCallTime && EntityTime < StunCallTime+StunDuration; } set { } }
    public bool Active {get{ return IsChannelling || IsActing || IsLagging;} }
    public double EntityTime = 0;
    double StunCallTime = 0;
    float StunDuration = 0;
    public override void _Ready()
	{
		base._Ready();
        foreach (Limb l in StartingLimbs)
        {
            Limbs.Add(l);
            l.Parent = this;
        }
        //GD.Print(Limbs.Count+" "+StartingLimbs.Length);
        TargetPos.GlobalPosition = GlobalPosition;
        //GD.Print(Limbs.Count);
        SetCenters();
	}
    public override void _Process(double delta) 
    {
        base._Process(delta);
        delta *= BulletTime.SpeedScale;
        // GD.Print("IsChannelling : "+IsChannelling+"\n"+
        //          "IsActing      : "+IsActing+"\n"+
        //          "IsLagging     : "+IsLagging+"\n"+
        //          "Active        : "+Active+"\n");

        
        VisualHP.Size = new Vector2(CurrentEnergy * 10,40);

        if (InvulnTimeRemaining > 0)
            InvulnTimeRemaining -= delta;

        if (RewindController.Instance.IsRewinding)
			return;

        //GD.Print(Animation.ActionAnimator.CurrentAnimation);
    }

    public override void _PhysicsProcess(double delta)
    {
        delta *= BulletTime.SpeedScale;
        SetCenters();

        EntityTime += delta;


        processMovement(delta);
        rotateTowardTarget(delta);
        if (IsStunned)
            stunned(delta);
    }
    protected virtual void postVelocityCalculation()
    {
        // for child classes to insert stuff after velocity calcuation
        // otherwise does nothing
    }

    protected void processMovement(double delta)
    {
        processGravity(delta);
        TargetPos.Velocity *= BulletTime.SpeedScale;
        TargetPos.MoveAndSlide();
        Vector3 vec = (TargetPos.GlobalPosition - GlobalPosition);
        Vector3 dir = vec.Normalized();
        if (TargetPos.IsOnFloor())
        {
            Speed = Math.Min(MaxWalkSpeed, vec.Length() * WalkLerpWeight);

        }
        else
        {
            Speed = vec.Length() / (float) delta;
        }
        Velocity = dir * Speed;
        Velocity *= BulletTime.SpeedScale;
        postVelocityCalculation();
        //GD.Print(Velocity);
        //GD.Print(AirTime);
        //GD.Print(Velocity + " "+TargetPos+" "+Speed+" "+Position+" "+ (TargetPos - GlobalPosition).Normalized());
        bool res = MoveAndSlide();

        //if (res)
        //{
        //    processBounce();
        //}
        TargetPos.Velocity = Vector3.Zero;
    }

    //protected void processBounce()
    //{
    //    Vector3 vec = (TargetPos.GlobalPosition - GlobalPosition);
    //    Vector3 deltaTargetPos = Vector3.Zero;
    //    for (int i = 0; i < GetSlideCollisionCount(); i++)
    //    {
    //        KinematicCollision3D col = GetSlideCollision(i);
    //        //GD.Print(col.GetAngle() + " " + FloorMaxAngle);
    //        if (col.GetAngle() < FloorMaxAngle)
    //            continue;
    //        //GD.Print("col " + col.GetCollider());
    //        Vector3 repel = col.GetNormal() * Repel;
    //        Vector3 bounce = vec.Bounce(col.GetNormal()) * resistance;
    //        deltaTargetPos += repel + bounce;

    //    }
    //    if (!deltaTargetPos.Equals(Vector3.Zero))
    //        TargetPos.GlobalPosition = GlobalPosition + deltaTargetPos;
    //}

    protected void processGravity(double delta)
    {
        if (TargetPos.IsOnFloor())
            AirTimeStart = EntityTime;
        float magnitude = -1;
        if (TargetPos.Velocity.Y + RisingGravity * Math.Max(0, EntityTime - AirTimeStart) > 0)
            magnitude = RisingGravity;
        else
            magnitude = FallingGravity;
        //GD.Print(magnitude);
        //GD.Print(TargetPos.Velocity.Y+" "+RisingGravity * Math.Max(0, EntityTime - AirTimeStart));
        TargetPos.Velocity += new Vector3(0, (float)(magnitude * Math.Max(0,EntityTime-AirTimeStart)), 0);
    }



    public virtual Vector3 TakeKnockback(Vector3 Force)
    {
        ////GD.Print("KNOCKBACK NOT IMPLEMENTED");
        //KinematicCollision3D col = MoveAndCollide(Force, true);
        ////GD.Print(col);
        //if (col != null)
        //    TargetPos += col.GetTravel();
        //else
        //    TargetPos += Force;
        //GD.Print("KNOCKBACK FORCE " + Force);
        //TargetPos += Force;
        if (IsInvuln)
            return Vector3.Zero;
        CenterOfMass.GlobalPosition += Force;
        
        return Force;
    }
    public virtual float DealDamage(Area3D other, float damage)
    {
        GD.Print("CALLED DEAL DAMAGER" + other.Name + " " + damage);
        return damage;
    }
    public virtual float TakeHit(float Damage)
    {
        if (IsInvuln)
            return 0;
        CurrentEnergy -= Damage;
        LastHitTime = EntityTime;
        //InvulnTimeRemaining = .1f;

        return Damage;
    }

    public virtual float TakeStunned(float Duration, bool selfInflicted = false)
    {
        if (IsStunned || (IsInvuln && !selfInflicted) || CurrentAction != null && !CurrentAction.Interrupt())
            return 0;
        StunCallTime = EntityTime;
        StunDuration = Duration;

        Animation?.EndAnimation();
       
        
        
        return Duration;
    }
    public virtual void stunned(double delta)
    {
        ;
    }
    public virtual void SetCenters()
    {
        setMassCenter();
        setStability();

        float weight = 0;
        if (IsStunned)
            weight = SlowRecoveryWeight;
        else
            weight = QuickRecoveryWeight;
        CenterOfMass.GlobalPosition = CenterOfMass.GlobalPosition.Lerp(CenterOfMassBase.GlobalPosition, weight);

        //GD.Print(Stability + " " +IsStunned );
    }
    public void processFall(float globalDeg)
    {
        //if (Animation != null)
        //    GD.Print(Animation.ActionAnimator.CurrentAnimation+" "+IsStunned);  
        if (IsStunned)
            return;
        
        if (Stability < FallStabilityThreshold)
        {
            TakeStunned(FallStunDuration, selfInflicted:true);

            // plays the fall animation with the closest angle to the actual direction
            int fallAng = FallDirections[0];
            double closestAngle = 180;
            float localDeg = (float)(globalDeg - GlobalRotation.Y/Math.PI*180 + 360) % 360;
            //GD.Print(dirt +" "+directionDegrees+" "+GlobalRotation.Y);
            foreach (int dir in FallDirections)
            {
                double angle = Math.Min(Math.Abs(dir- localDeg), 360 - Math.Abs(dir - localDeg));
                //GD.Print(dir + " " + angle);
                if (angle < closestAngle)
                {
                    closestAngle = angle;
                    fallAng = dir;
                }
            }
            Animation?.Play("Fall" + fallAng);

            // rotates so the falling animation matches actual direction
            if (localDeg > fallAng)
                RotateY((float)(closestAngle / 180 * Math.PI));
            else
                RotateY(-(float)(closestAngle / 180 * Math.PI));

            // knocks back in proportion to how far the center of mass was moved
            Vector3 fallDir = (Vector3.Forward.Rotated(Vector3.Up, (float)(globalDeg/180*Math.PI))).Normalized();
            //GD.Print(globalDeg);

            TargetPos.MoveAndCollide(fallDir * (1-Stability) * FallMaxDistance);

        }
        else if (Stability < StumbleStabilityThreshold)
        {
            TakeStunned(StumbleStunDuration, selfInflicted: true);
            Animation?.Play("Stumble");
        }
        
    }
    public virtual void setStability()
    {
        List<Vector3> legs = new List<Vector3>();
        foreach (Limb L in Limbs)
        {
            if (L.IsFooting)
            {
                legs.Add((L.GlobalPosition - CenterOfMass.GlobalPosition).Normalized());
                //GD.Print((L.GlobalPosition - CenterOfMass.GlobalPosition).Normalized());
            }
        }

        float pi = (float)Math.PI;
        float maxClosestAngle = 0;
        float dir = 0;
        for (int ang = 0; ang < 360; ang++) 
        {
            Vector3 vec = Vector3.Forward.Rotated(Vector3.Up, ang * pi / 180f);
            //GD.Print(ang * pi / 180f);
            //GD.Print(vec);
            float closestAngle = pi;
            foreach (Vector3 limb in legs)
            {
               closestAngle = Math.Min(closestAngle, vec.AngleTo(limb));
            }

            if (closestAngle > maxClosestAngle)
                dir = ang;
            maxClosestAngle = Math.Max(maxClosestAngle, closestAngle);
        }
        //GD.Print(legs.Count);
        Stability = (pi - maxClosestAngle) / pi;
        //GD.Print(dir);
        processFall(dir);
    }

    public virtual void setMassCenter()
    {
        Vector3 MassCenter = Vector3.Zero;
        float mCount = 0;
        foreach (Limb L in Limbs)
        {
            MassCenter += L.GlobalPosition * L.MassFactor;
            mCount += L.MassFactor;
        }
        if (mCount > 0)
            MassCenter /= mCount;
        else
            MassCenter = GlobalPosition;

        Vector3 delta = MassCenter - CenterOfMassBase.GlobalPosition;
        CenterOfMassBase.GlobalPosition += delta;
        CenterOfMass.GlobalPosition += delta;   
    }

    public virtual void rotateTowardTarget(double delta)
    {
        //GD.Print("going");
        TrackingProperties currentTrackingProperties = TrackingPropertiesBase;
        if (CurrentAction is ITrackingChange && (CurrentAction as ITrackingChange).TrackingProperties != null)
            currentTrackingProperties = (CurrentAction as ITrackingChange).TrackingProperties;

        if (!IsStunned)    
        {
            float anglePerSecond = 0f;
            Vector3 start = new Vector3(GlobalBasis[2][0], 0, GlobalBasis[2][2]).Normalized();
            Vector3 end = new Vector3(TargetPos.GlobalBasis[2][0], 0, TargetPos.GlobalBasis[2][2]).Normalized();
            float angleTo = start.SignedAngleTo(end, Vector3.Up);
            if (currentTrackingProperties.SlerpTracking)
            {
                anglePerSecond = Math.Min(Math.Abs(angleTo * currentTrackingProperties.SlerpWeight), currentTrackingProperties.MaximumRotationPerSecond);

                if (angleTo < 0)
                    anglePerSecond = -anglePerSecond;
            }
            else
            {
                if (angleTo > 0)
                    anglePerSecond = currentTrackingProperties.LinearRotationPerSecond;
                else
                    anglePerSecond = -currentTrackingProperties.LinearRotationPerSecond;
            }
            //GD.Print(angle);
            //GD.Print(cross);
            if (Math.Abs(anglePerSecond * delta) > Math.Abs(angleTo))
                GlobalBasis = GlobalBasis.Rotated(Vector3.Up, angleTo);
            else
                GlobalBasis = GlobalBasis.Rotated(Vector3.Up, (float)(anglePerSecond * delta));
        }

    }
    public int DataLength{get{return 4;}}
    public virtual List<Object> GetData()
    {
        List<Object> data = new List<Object>
        {
            CurrentEnergy,
            MaxEnergy,
            CurrentAction,
            TargetPos.GlobalPosition
        };
		return data;
    }

    public virtual void SetData(List<Object> data)
    {
		CurrentEnergy      = (float)   data[0];
		MaxEnergy          = (float)   data[1];
        CurrentAction  = (Action)  data[2];
        TargetPos.GlobalPosition      = (Vector3) data[3];
    }

}

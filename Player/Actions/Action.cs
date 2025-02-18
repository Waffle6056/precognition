using Godot;
using System;
using System.Collections.Generic;
using System.Dynamic;

public partial class Action : Node3D, RewindableObject, ActionState
{
	[Export]
	public float ChannelTimeBase = 0;
	[Export]
	public float EndLagTimeBase = 0;
	[Export]
	public String CallKeyBind;
	[Export]
	public AnimationPlayer ActionAnimator;
	[Export]
	public Entity Root;
	[Export]
	public CooldownManager Cooldown;
	[Export]
	public bool Interruptable = true;
	protected double ChannelTime = 0;
	protected double EndLagTime = 0;
	protected double ActionTime = 0.0;
	public bool IsChannelling{get; set;}
	public bool IsActing{get; set;}
	public bool IsLagging{get; set;}
	public bool Active {get{ return IsChannelling || IsActing || IsLagging;} }
	public bool OnCD() { return Cooldown == null ? false : Cooldown.OnCD(); }

	protected virtual bool StartChannel()
	{
		ChannelTime = ChannelTimeBase;
		Root.IsChannelling = true;
		IsChannelling = true;
		return true;
		
		//GD.Print("Channel Started");
	}
	protected virtual bool Channel(double delta)
	{
		ChannelTime -= delta;
		if (ChannelTime <= 0)
			EndChannel();
		return true;
	}

	protected virtual bool EndChannel()
	{
		Root.IsChannelling = false;
		IsChannelling = false;

		//GD.Print("Channel Finished");

		StartAction();
		return true;
	}

	protected virtual bool StartAction()
	{
		Root.IsActing = true;
		IsActing = true;

		//GD.Print("BaseAttack Called");
		return true;
	}
	protected virtual bool Act(double delta)
	{
		return true;
	}
	protected virtual bool EndAction()
	{
		//GD.Print("ACTION ENDED");
		Root.IsActing = false;
		IsActing = false;
		StartEndLag();
		return true;
	}
	protected virtual bool StartEndLag()
	{
		EndLagTime = EndLagTimeBase;
		Root.IsLagging = true;
		IsLagging = true;

		//GD.Print("BaseAttack Called");
		return true;
		
	}
	protected virtual bool Lag(double delta)
	{
		EndLagTime -= delta;
		if (EndLagTime <= 0)
			EndEndLag();
		return true;
	}
	protected virtual bool EndEndLag()
	{
		Root.IsLagging = false;
		IsLagging = false;
		Cooldown?.Start();
		return true;
	}


	public bool CallAction()
	{
		//GD.Print("Attack Called");
		if (OnCD() || Root.Active || Active)
			return false;
		return StartChannel();
		
	}
	
	
	// returns false if interrupted ie returns true if executed without issue
	public virtual bool Update(double delta){ 
		if (RewindController.Instance.IsPaused)
			return false;

		ActionTime += delta;

		if (CallKeyBind != null && CallKeyBind.Length > 0 && Input.IsActionJustPressed(CallKeyBind))
			CallAction();

		if (IsChannelling)
			Channel(delta);

		if (IsActing)
			Act(delta);

		if (IsLagging)
			Lag(delta);

		return true;
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{	
		
		
		base._Process(delta);
		Update(delta);

	}

    public int DataLength{get{return 8;}}
	public virtual List<Object> GetData()
    {
        List<Object> data = new List<Object>
        {
			IsChannelling,
			IsActing,
			IsLagging,
			ChannelTime,
			EndLagTime,
			ActionTime,
			ActionAnimator != null && ActionAnimator.IsPlaying() ? ActionAnimator.CurrentAnimation : "",
            ActionAnimator != null && ActionAnimator.IsPlaying() ? ActionAnimator.CurrentAnimationPosition : 0.0,
			
        };
		return data;
    }

    public virtual void SetData(List<Object> data)
    {
		IsChannelling = (bool)   data[0];
		IsActing      = (bool)   data[1];
		IsLagging     = (bool)   data[2];
		ChannelTime   = (double) data[3];
		EndLagTime    = (double) data[4];
		ActionTime    = (double) data[5];
		if (ActionAnimator != null){
			ActionAnimator.CurrentAnimation = (String)data[6];
			ActionAnimator.Seek((double)data[7], true);
		}
		
	}
}

interface ActionState
{
	public bool IsChannelling{get; set;}
	public bool IsActing{get; set;}
	public bool IsLagging{get; set;}
	
	public bool Active {get{ return IsChannelling || IsActing || IsLagging;} }
}
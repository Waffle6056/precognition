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
	protected double ChannelTime = 0;
	protected double EndLagTime = 0;
	public bool IsChannelling{get; set;}
	public bool IsActing{get; set;}
	public bool IsLagging{get; set;}
	public bool Active {get{ return IsChannelling || IsActing || IsLagging;} }
	public bool OnCD() { return Cooldown == null ? false : Cooldown.OnCD(); }

	protected virtual void StartChannel()
	{
		ChannelTime = ChannelTimeBase;
		Root.IsChannelling = true;
		IsChannelling = true;
		
		//GD.Print("Channel Started");
	}
	protected virtual void Channel(double delta)
	{
		ChannelTime -= delta;
		if (ChannelTime <= 0)
			EndChannel();
	}

	protected virtual void EndChannel()
	{
		Root.IsChannelling = false;
		IsChannelling = false;

		//GD.Print("Channel Finished");

		StartAction();
	}

	protected virtual void StartAction()
	{
		Root.IsActing = true;
		IsActing = true;

		//GD.Print("BaseAttack Called");
	}
	protected virtual void Act(double delta)
	{
		;
	}
	protected virtual void EndAction()
	{
		Root.IsActing = false;
		IsActing = false;
		StartEndLag();
	}
	protected virtual void StartEndLag()
	{
		EndLagTime = EndLagTimeBase;
		Root.IsLagging = true;
		IsLagging = true;

		//GD.Print("BaseAttack Called");
	}
	protected virtual void Lag(double delta)
	{
		EndLagTime -= delta;
		if (EndLagTime <= 0)
			EndEndLag();
	}
	protected virtual void EndEndLag()
	{
		Root.IsLagging = false;
		IsLagging = false;
		Cooldown?.Start();
	}


	public void CallAction()
	{
		//GD.Print("Attack Called");
		if (OnCD() || Root.Active || Active)
			return;
		StartChannel();
		
	}
	
	

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{	
		base._Process(delta);
		if (RewindController.Instance.IsPaused)
			return;



		if (CallKeyBind != null && CallKeyBind.Length > 0 && Input.IsActionJustPressed(CallKeyBind))
			CallAction();

		if (IsChannelling)
			Channel(delta);

		if (IsActing)
			Act(delta);

		if (IsLagging)
			Lag(delta);

	}

    public int DataLength{get{return 7;}}
	public virtual List<Object> GetData()
    {
        List<Object> data = new List<Object>
        {
			IsChannelling,
			IsActing,
			IsLagging,
			ChannelTime,
			ActionAnimator != null && ActionAnimator.IsPlaying() ? ActionAnimator.CurrentAnimation : "",
            ActionAnimator != null && ActionAnimator.IsPlaying() ? ActionAnimator.CurrentAnimationPosition : 0.0,
			EndLagTime,
        };
		return data;
    }

    public virtual void SetData(List<Object> data)
    {
		IsChannelling = (bool)   data[0];
		IsActing      = (bool)   data[1];
		IsLagging     = (bool)   data[2];
		ChannelTime   = (double) data[3];
		if (ActionAnimator != null){
			ActionAnimator.CurrentAnimation = (String)data[4];
			ActionAnimator.Seek((double)data[5], true);
		}
		EndLagTime   = (double) data[6];
	}
}

interface ActionState
{
	public bool IsChannelling{get; set;}
	public bool IsActing{get; set;}
	public bool IsLagging{get; set;}
	
	public bool Active {get{ return IsChannelling || IsActing || IsLagging;} }
}
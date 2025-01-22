using Godot;
using System;
using System.Collections.Generic;
using System.Dynamic;

public partial class Action : Node3D, RewindableObject
{
	[Export]
	public float ChannelTimeBase = 0;
	[Export]
	public String CallKeyBind;
	[Export]
	public AnimationPlayer ActionAnimator;
	[Export]
	public Entity Root;
	[Export]
	public CooldownManager Cooldown;
	protected double ChannelTime = 0;
	protected bool IsChannelling = false;
	protected bool IsActing = false;
	public bool OnCD() { return Cooldown == null ? false : Cooldown.OnCD(); }
	


	protected virtual void StartChannel()
	{
		ChannelTime = ChannelTimeBase;
		Root.Channelling = true;
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
		Root.Channelling = false;
		IsChannelling = false;

		//GD.Print("Channel Finished");

		StartAction();
	}

	protected virtual void StartAction()
	{
		Root.Acting = true;
		IsActing = true;

		//GD.Print("BaseAttack Called");
	}
	protected virtual void Act(double delta)
	{
		;
	}
	protected virtual void EndAction()
	{
		Root.Acting = false;
		IsActing = false;
		Cooldown?.Start();
	}


	public void CallAction()
	{
		//GD.Print("Attack Called");
		if (OnCD() || Root.Channelling || Root.Acting || IsChannelling || IsActing)
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

	}

    public int DataLength{get{return 5;}}
	public virtual List<Object> GetData()
    {
        List<Object> data = new List<Object>
        {
			IsChannelling,
			IsActing,
			ChannelTime,
			ActionAnimator != null && ActionAnimator.IsPlaying() ? ActionAnimator.CurrentAnimation : "",
            ActionAnimator != null && ActionAnimator.IsPlaying() ? ActionAnimator.CurrentAnimationPosition : 0.0,
        };
		return data;
    }

    public virtual void SetData(List<Object> data)
    {
		IsChannelling = (bool)   data[0];
		IsActing      = (bool)   data[1];
		ChannelTime   = (double) data[2];
		if (ActionAnimator != null){
			ActionAnimator.CurrentAnimation = (String)data[3];
			ActionAnimator.Seek((double)data[4], true);
		}
	}
}
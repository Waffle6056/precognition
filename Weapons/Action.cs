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
	protected bool Channelling = false;
	protected bool Acting = false;
	public bool OnCD() { return Cooldown == null ? false : Cooldown.OnCD(); }
	


	protected virtual void StartChannel()
	{
		ChannelTime = ChannelTimeBase;
		Root.Channelling = true;
		Channelling = true;
		
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
		Channelling = false;

		//GD.Print("Channel Finished");

		StartAction();
	}

	protected virtual void StartAction()
	{
		Root.Acting = true;
		Acting = true;

		//GD.Print("BaseAttack Called");
	}
	protected virtual void Act(double delta)
	{
		;
	}
	protected virtual void EndAction()
	{
		Root.Acting = false;
		Acting = false;
		Cooldown?.Start();
	}


	public void CallAction()
	{
		//GD.Print("Attack Called");
		if (OnCD() || Root.Channelling || Root.Acting || Channelling || Acting)
			return;
		StartChannel();
		
	}
	
	

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{	
		if (Channelling)
			Channel(delta);
		
		
		if (CallKeyBind != null && CallKeyBind.Length > 0 && Input.IsActionJustPressed(CallKeyBind))
			CallAction();

		if (Acting)
			Act(delta);

	}


	protected static int DataLength = 5;
	public virtual List<Object> GetData()
    {
        List<Object> data = new List<Object>
        {
			Channelling,
			Acting,
			ChannelTime,
			ActionAnimator != null && ActionAnimator.IsPlaying() ? ActionAnimator.CurrentAnimation : "",
            ActionAnimator != null && ActionAnimator.IsPlaying() ? ActionAnimator.CurrentAnimationPosition : 0.0,
        };
		return data;
    }

    public virtual void SetData(List<Object> data)
    {
		Channelling     = (bool)   data[0];
		Acting       = (bool)   data[1];
		ChannelTime     = (double) data[2];
		if (ActionAnimator != null){
			ActionAnimator.CurrentAnimation = (String)data[3];
			ActionAnimator.Seek((double)data[4], true);
		}
	}
}
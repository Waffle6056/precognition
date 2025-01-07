using Godot;
using System;
using System.Collections.Generic;
using System.Dynamic;

public partial class Weapon : Node3D, RewindableObject
{
	[Export]
	public float CDBase = 0;
	[Export]
	public float ChannelTimeBase = 0;
	[Export]
	public String CallKeyBind;
	[Export]
	public AnimationPlayer AttackAnimator;
	[Export]
	public Entity Root;
	protected double CD = 0;
	protected double ChannelTime = 0;
	protected bool Channelling = false;
	protected bool Attacking = false;
	public bool OnCD() { return CD > 0; }
	


	protected virtual void StartChannel()
	{
		ChannelTime = ChannelTimeBase;
		Root.Channelling = true;
		Channelling = true;
		
		GD.Print("Channel Started");
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

		GD.Print("Channel Finished");

		StartAttack();
	}

	protected virtual void StartAttack()
	{
		Root.Attacking = true;
		Attacking = true;

		GD.Print("BaseAttack Called");
	}
	protected virtual void Attack(double delta)
	{
		;
	}
	protected virtual void EndAttack()
	{
		Root.Attacking = false;
		Attacking = false;
	}


	public void CallAttack()
	{
		GD.Print("Attack Called");
		if (OnCD())
			return;
		CD = CDBase;
		StartChannel();
		
	}
	
	

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{	
		if (Channelling)
			Channel(delta);
		
		
		if (CallKeyBind != null && CallKeyBind.Length > 0 &&Input.IsActionJustPressed(CallKeyBind))
			CallAttack();

		if (Attacking)
			Attack(delta);

		if (!Channelling && !Attacking)
			CD -= delta;
	}


	protected static int DataLength = 8;
	public virtual List<Object> GetData()
    {
        List<Object> data = new List<Object>
        {
			Channelling,
			Attacking,
			ChannelTime,
			ChannelTimeBase,
            CD,
			CDBase,
			AttackAnimator.IsPlaying() ? AttackAnimator.CurrentAnimation : "",
            AttackAnimator.IsPlaying() ? AttackAnimator.CurrentAnimationPosition : 0.0,
        };
		return data;
    }

    public virtual void SetData(List<Object> data)
    {
		Channelling     = (bool)   data[0];
		Attacking       = (bool)   data[1];
		ChannelTime     = (double) data[2];
		ChannelTimeBase = (float)  data[3];
        CD              = (double) data[4];
		CDBase          = (float)  data[5];
		AttackAnimator.CurrentAnimation = (String)data[6];
		AttackAnimator.Seek((double)data[7], true);
    }
}
using Godot;
using System;
using System.Collections.Generic;
using System.Dynamic;

public partial class Action : Option, RewindableObject, ActionState
{
	[Export]
	public String CallKeyBind;
	[Export]
	public CooldownManager Cooldown;
	[Export]
	public ActionProperties Properties;
	protected double ChannelCallTime = 0;
	protected double EndLagCallTime = 0;
	protected double ActionCallTime = 0;
	protected double ActionTime = 0.0;
	public bool IsChannelling{get; set;}
	public bool IsActing{get; set;}
	public bool IsLagging{get; set;}
	public bool Active {get{ return IsChannelling || IsActing || IsLagging;} }
	public bool OnCD() { return Cooldown == null ? false : Cooldown.OnCD(); }

	protected virtual bool StartChannel()
	{
		ChannelCallTime = ActionTime;
		IsChannelling = true;
		return true;
		
		//GD.Print("Channel Started");
	}
	protected virtual bool Channel(double delta)
	{
		if (ActionTime >= ChannelCallTime + Properties.ChannelTime)
			EndChannel();
		return true;
	}

	protected virtual bool EndChannel()
	{
		IsChannelling = false;

		//GD.Print("Channel Finished");

		StartAction();
		return true;
	}

	protected virtual bool StartAction()
	{
		ActionCallTime = ActionTime;
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
		IsActing = false;
		StartEndLag();
		return true;
	}
	protected virtual bool StartEndLag()
	{
		EndLagCallTime = ActionTime;
		IsLagging = true;

		//GD.Print("BaseAttack Called");
		return true;
		
	}
	protected virtual bool Lag(double delta)
	{
		if (ActionTime >= EndLagCallTime + Properties.EndLagTime)
			EndEndLag();
		return true;
	}
	protected virtual bool EndEndLag()
	{
		IsLagging = false;
		Cooldown?.Start();
		return true;
	}


	public bool CallAction()
	{
		//GD.Print("Attack Called");
		if (OnCD() || Properties.Root.Active || Active)
			return false;
		if (StartChannel()){
			Properties.Root.CurrentAction = this;
			return true;
		}
		return false;
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
	public virtual bool SetTime(double Time){
		return Update(Time-ActionTime);
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{	
		base._Process(delta);
		Update(delta);

	}

	public override void LoadActions(){
        options = new Action[]{this};
    }


    public int DataLength{get{return 6;}}
	public virtual List<Object> GetData()
    {
        List<Object> data = new List<Object>
        {
			IsChannelling,
			IsActing,
			IsLagging,
			ChannelCallTime,
			EndLagCallTime,
			ActionCallTime,
        };
		return data;
    }

    public virtual void SetData(List<Object> data)
    {
		IsChannelling = (bool)   data[0];
		IsActing      = (bool)   data[1];
		IsLagging     = (bool)   data[2];
		ChannelCallTime   = (double) data[3];
		EndLagCallTime    = (double) data[4];
		ActionCallTime    = (double) data[5];
		
		
	}
}

interface ActionState
{
	public bool IsChannelling{get; set;}
	public bool IsActing{get; set;}
	public bool IsLagging{get; set;}
	
	public bool Active {get{ return IsChannelling || IsActing || IsLagging;} }
}
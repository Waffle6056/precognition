using Godot;
using System;
using System.Collections.Generic;
using System.Dynamic;

[GlobalClass]
public partial class Action : Option, RewindableObject, ActionState
{
	//[Export]
	//public String[] CallKeyBind;
	[Export]
	public CooldownManager Cooldown;
	[Export]
	public ActionProperties ActionProperties;
	protected double ChannelCallTime = 0;
	protected double EndLagCallTime = 0;
	protected double ActionCallTime = 0;
	protected double ActionTime = 0.0;
    public double CurrentActionBufferTime = 0;

    public bool	StartedChannelling {get; set;}
    public bool IsChannelling{get; set; }
    public bool EndedChannelling { get; set; }
    public bool StartedActing { get; set; }
    public bool IsActing{get; set; }
    public bool EndedActing { get; set; }
    public bool StartedLagging { get; set; }
    public bool IsLagging{get; set; }
    public bool EndedLagging { get; set; }
    public bool Active {get{ return IsChannelling || IsActing || IsLagging;} }
	public bool OnCD() { return Cooldown == null ? false : Cooldown.OnCD(); }

	protected virtual bool StartChannel()
	{
		ChannelCallTime = ActionTime;
		IsChannelling = true;
		StartedChannelling = true;
		return true;
		
		//GD.Print("Channel Started");	
	}
	protected virtual bool Channel(double delta)
	{
		if (ActionTime >= ChannelCallTime + ActionProperties.ChannelTime)
			EndChannel();
		return true;
	}

	protected virtual bool EndChannel()
	{
		IsChannelling = false;
        EndedChannelling = true;

        //GD.Print("Channel Finished");

        StartAction();
		return true;
	}

	protected virtual bool StartAction()
	{
		ActionCallTime = ActionTime;
		IsActing = true;
        StartedActing = true;

        //GD.Print("BaseAttack Called");
        return true;
	}
	protected virtual bool Act(double delta)
	{
        if (ActionTime >= ActionCallTime + ActionProperties.ActingMaximumTime)
            EndAction();
        return true;
	}
	protected virtual bool EndAction()
	{
		//GD.Print("ACTION ENDED");
		IsActing = false;
        EndedActing = true;
        StartEndLag();
		return true;
	}
	protected virtual bool StartEndLag()
	{
		EndLagCallTime = ActionTime;
		IsLagging = true;
        StartedLagging = true;

        //GD.Print("BaseAttack Called");
        return true;
		
	}
	protected virtual bool Lag(double delta)
	{
		if (ActionTime >= EndLagCallTime + ActionProperties.EndLagTime)
			EndEndLag();
		return true;
	}
	protected virtual bool EndEndLag()
	{
		IsLagging = false;
        EndedLagging = true;
        Cooldown?.Start();
		return true;
	}
	public virtual bool Interrupt()
	{
		if (!Active)
			return false;

		IsChannelling = false;
        IsActing = false;
        IsLagging = false;

        if (this is IAnimated)
			(this as IAnimated).Animation?.EndAnimation();

		return true;
	}
	
	public bool CallAction()
	{
		//GD.Print("Attack Called "+Name+" "+( Active));
		if (OnCD() || Active)
			return false;
		if (StartChannel()){
			return true;
		}
		return false;
	}
	
	
	// returns false if interrupted ie returns true if executed without issue
	public virtual bool Update(double delta){ 
		if (RewindController.Instance.IsPaused)
			return false;

		StartedChannelling	= false;
		EndedChannelling = false;
        StartedActing = false;
        EndedActing = false;
        StartedLagging = false;
        EndedLagging = false;


        ActionTime += delta;

		//if (CallKeyBind != null && CallKeyBind.Length > 0)
		//{
		//	bool good = true;
		//	foreach (String Bind in CallKeyBind)
		//	{
		//		String[] query = Bind.Split(',');
		//		bool justP = Input.IsActionJustPressed(query[0]);
  //              bool contP = Input.IsActionPressed(query[0]);

		//		if (query.Length > 1 && query[1].Equals("JustPressed")) 
		//			good &= justP;
		//		else
		//			good &= contP;
		//	}
		//	if (good)
		//		CallAction();
		//}

		if (IsChannelling)
			Channel(delta);

		if (IsActing)
			Act(delta);

		if (IsLagging)
			Lag(delta);

        if (CurrentActionBufferTime > 0)
            CurrentActionBufferTime -= delta;
        if (EndedActing)
            CurrentActionBufferTime = ActionProperties.ActionBufferTime;

        return true;
	}
	public virtual bool SetTime(double Time){
		return Update(Time-ActionTime);
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{	
		base._Process(delta);
		Update(delta * BulletTime.SpeedScale);

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
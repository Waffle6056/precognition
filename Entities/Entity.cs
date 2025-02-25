using Godot;
using System;
using System.Collections.Generic;

public partial class Entity : AnimatableBody3D, RewindableObject, ActionState
{
    [Export]
	public float MaxHP{get; set;} = 100;
	[Export]
	public float CurrentHP{get; set;} = 100;
    [Export]
	public ColorRect VisualHP;
    [Export]
	public AnimatableBody3D GridSpace;
    [Export]
	public bool LerpOn = true;
    [Export]
	public float Weight = .5f;
    [Export]
    public Action CurrentAction;
    public bool IsChannelling{get{ return CurrentAction!=null && CurrentAction.IsChannelling; } set{}}
	public bool IsActing{get{ return CurrentAction!=null && CurrentAction.IsActing; } set{}}
	public bool IsLagging{get{ return CurrentAction!=null && CurrentAction.IsLagging; } set{}}
	public bool Active {get{ return IsChannelling || IsActing || IsLagging;} }
    public float Time = 0;
    public Vector3 TargetPos = new Vector3(0,0,0);
    public override void _Ready()
	{
		base._Ready();
        TargetPos = Position;
	}
    public override void _Process(double delta) {
        base._Process(delta);
        // GD.Print("IsChannelling : "+IsChannelling+"\n"+
        //          "IsActing      : "+IsActing+"\n"+
        //          "IsLagging     : "+IsLagging+"\n"+
        //          "Active        : "+Active+"\n");
        if (LerpOn)
			Position = Position.Lerp(TargetPos,Weight);
		else
			Position = TargetPos;
		GridSpace.GlobalPosition = TargetPos;
        VisualHP.Size = new Vector2(CurrentHP,40);

        if (RewindController.Instance.IsRewinding)
			return;
    }
    public virtual float TakeHit(float Damage){
        CurrentHP -= Damage;
        return Damage;
    }

    
    public int DataLength{get{return 4;}}
    public virtual List<Object> GetData()
    {
        List<Object> data = new List<Object>
        {
            CurrentHP,
            MaxHP,
            CurrentAction,
            TargetPos
        };
		return data;
    }

    public virtual void SetData(List<Object> data)
    {
		CurrentHP      = (float)   data[0];
		MaxHP          = (float)   data[1];
        CurrentAction  = (Action)  data[2];
        TargetPos      = (Vector3) data[3];
    }

}

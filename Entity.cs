using Godot;
using System;
using System.Collections.Generic;

public partial class Entity : AnimatableBody3D, RewindableObject
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
	public bool Channelling = false;
	public bool Acting = false;
    protected static int DataLength = 5;
    public Vector3 TargetPos = new Vector3(0,0,0);
    public override void _Ready()
	{
		base._Ready();
        TargetPos = Position;
	}
    public override void _Process(double delta) {
        base._Process(delta);
        if (LerpOn)
			Position = Position.Lerp(TargetPos,Weight);
		else
			Position = TargetPos;
		GridSpace.GlobalPosition = TargetPos;
        VisualHP.Size = new Vector2(CurrentHP,40);
    }
    public virtual float TakeHit(float Damage){
        CurrentHP -= Damage;
        return Damage;
    }
    public virtual List<Object> GetData()
    {
        List<Object> data = new List<Object>
        {
            CurrentHP,
            MaxHP,
			Acting,
			Channelling,
            GlobalPosition
        };
		return data;
    }

    public virtual void SetData(List<Object> data)
    {
		CurrentHP   = (float)   data[0];
		MaxHP       = (float)   data[1];
		Acting   = (bool)    data[2];
		Channelling = (bool)    data[3];
        GlobalPosition = (Vector3) data[4];
    }

}

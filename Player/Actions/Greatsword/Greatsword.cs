using Godot;
using System;
using System.Collections.Generic;

public partial class Greatsword : AttackAction
{

	String AttackName = "Swing";
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);
	}
	protected override void StartChannel()
	{
		base.StartChannel();
		ActionAnimator.Play(AttackName);
	}
    protected override void EndChannel()
    {
        base.EndChannel();
		EndAction();
    }


}

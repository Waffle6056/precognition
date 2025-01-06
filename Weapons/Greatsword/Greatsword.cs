using Godot;
using System;
using System.Collections.Generic;

public partial class Greatsword : Weapon
{

	String AttackName = "Swing";
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);
	}

	protected override void StartAttack()
	{
		base.StartAttack();
		GD.Print("AttackStarted");
		AttackAnimator.Play(AttackName);
	}
	public void AnimationEnd(String name){
		if (Attacking)
			AttackAnimator.PlayBackwards(AttackName);
		EndAttack();		
	}
	
	
	public override List<Object> GetData()
    {		
        List<Object> data = base.GetData();
		GD.Print(AttackAnimator.CurrentAnimation);
		data.AddRange(new List<Object>
        {
            AttackAnimator.IsPlaying() ? AttackAnimator.CurrentAnimation : "",
            AttackAnimator.IsPlaying() ? AttackAnimator.CurrentAnimationPosition : 0.0,
        });
		return data;
    }

    public override void SetData(List<Object> data)
    {
		base.SetData(data);
		AttackAnimator.CurrentAnimation = (String)data[Weapon.DataLength + 0];
		AttackAnimator.Seek((double)data[Weapon.DataLength + 1], true);
    }
}

using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class AnimationManager : Node, RewindableObject
{
	[Export]
	public AnimationPlayer ActionAnimator{get;set;}
	public void Play(String Name)
	{
		ActionAnimator.Play(Name);
	}


	public void EndAnimation()
	{
		ActionAnimator.Advance(ActionAnimator.CurrentAnimationLength - ActionAnimator.CurrentAnimationPosition);
	}


    public int DataLength{get{return 2;}}
	public List<Object> GetData()
	{		
		List<Object> data = new List<Object>
		{
			ActionAnimator != null && ActionAnimator.IsPlaying() ? ActionAnimator.CurrentAnimation : "",
			ActionAnimator != null && ActionAnimator.IsPlaying() ? ActionAnimator.CurrentAnimationPosition : 0.0,
		};
		return data;
	}

	public void SetData(List<Object> data)
	{
		if (ActionAnimator != null){
			ActionAnimator.CurrentAnimation = (String)data[0];
			ActionAnimator.Seek(              (double)data[1], true);
		}
	}
}
interface IAnimated
{
	[Export]
	public AnimationManager Animation{get; set;}
}

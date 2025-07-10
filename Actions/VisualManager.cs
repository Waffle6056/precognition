using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class VisualManager : Node3D, RewindableObject
{
	[Export]
	public AnimationPlayer ActionAnimator{get;set; }
    [Export]
    public Node3D VisualRoot { get; set; }
    [Export]
    public Material GhostMaterial { get; set; }
    public void Play(String name)
	{
		ActionAnimator.Play(name);
	}
    public void Play(String name, float offset)
    {
		//GD.Print(name);
        ActionAnimator.Play(name);
		ActionAnimator.Advance(offset);
    }


    public void EndAnimation()
	{
		ActionAnimator.Advance(ActionAnimator.CurrentAnimationLength - ActionAnimator.CurrentAnimationPosition);
	}
	static void applyMaterial(Node root, Material m)
	{
		if (root is GeometryInstance3D)
			(root as GeometryInstance3D).MaterialOverride = m;
		foreach (Node child in root.GetChildren())
			applyMaterial(child, m);
	}
	public VisualManager PlayFuture(float offset)
	{
		//GD.Print(ActionAnimator.CurrentAnimation);
		return PlayFuture(offset, ActionAnimator.CurrentAnimation);
	}

    public VisualManager PlayFuture(float offset, String animName)
	{
		VisualManager dupe = this.Duplicate() as VisualManager;

        dupe.VisualRoot.ProcessMode = ProcessModeEnum.Disabled;
        applyMaterial(dupe.VisualRoot, GhostMaterial);

        dupe.Play(animName, offset);
		dupe.ActionAnimator.AnimationFinished += dupe.DeleteSignal;


        AddChild(dupe);
        return dupe;
	}
	public void DeleteSignal(StringName n)
	{
		QueueFree();
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
	public VisualManager Animation{get; set;}
}

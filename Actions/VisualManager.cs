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
	[Export]
	public float GhostSpeedScale = 3f;
    public double Hitstop = 0;
    //public double HitstopCache = 0;
    public void Play(String name)
	{
		if (ActionAnimator.IsPlaying())
		{
			EndAnimation();
			//GD.Print("a");
		}
		
		ActionAnimator.Play(name);
	}
    public void Play(String name, float offset, float speed)
    {
        if (ActionAnimator.IsPlaying())
            EndAnimation();
        //GD.Print(name);
        ActionAnimator.Play(name);
		ActionAnimator.Advance(offset);
        ActionAnimator.SpeedScale = speed;
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

	private VisualManager dupe;
    public VisualManager PlayFuture(float offset, String animName)
	{
        dupe = this.Duplicate() as VisualManager;

        dupe.VisualRoot.ProcessMode = ProcessModeEnum.Disabled;
        applyMaterial(dupe.VisualRoot, GhostMaterial);

        dupe.Play(animName, offset, GhostSpeedScale);
		dupe.ActionAnimator.AnimationFinished += dupe.DeleteSignal;


        AddChild(dupe);
        return dupe;
	}
    public void EndFuture()
    {
        dupe?.EndAnimation();
    }

    public void DeleteSignal(StringName n)
	{
		QueueFree();
	}
    public void HitStop(double amt)
    {
		Hitstop += amt;
        //HitstopCache += amt;
    }
    public override void _Process(double delta)
    {
        base._Process(delta);
		if (Hitstop > 0) {
			ActionAnimator.Pause();
			Hitstop -= delta;
			if (Hitstop < 0)
                ActionAnimator.Play();
        }
		//else
		//{
		//	//ActionAnimator.Advance(HitstopCache);
		//	//HitstopCache = 0;
		//}
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
